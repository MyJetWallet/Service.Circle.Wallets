using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Postgres;
using Service.Circle.Webhooks.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Circle.Wallets.Subscribers
{
    public class CircliCardSignalSubscriber
    {
        private readonly ILogger<CircliCardSignalSubscriber> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<CircleCardNoSqlEntity> _writer;

        public CircliCardSignalSubscriber(
            ILogger<CircliCardSignalSubscriber> logger,
            ISubscriber<SignalCircleCard> subscriber,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IMyNoSqlServerDataWriter<CircleCardNoSqlEntity> writer)
        {
            subscriber.Subscribe(HandleSignal);
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _writer = writer;
        }

        private async ValueTask HandleSignal(SignalCircleCard cardSignal)
        {
            try
            {
                _logger.LogInformation("Processing SignalCircleCard due to {error}", cardSignal.ToJson());

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var card = await ctx.Cards.FirstOrDefaultAsync(x => x.CircleCardId == cardSignal.CircleCardId);

                if (cardSignal.Verified)
                {
                    card.Status = CircleCardStatus.Complete;
                    card.UpdateDate = DateTime.UtcNow;
                }
                else
                {
                    card.Status = CircleCardStatus.Failed;
                    card.UpdateDate = DateTime.UtcNow;
                    card.IsActive = false;
                }

                await ctx.Cards.Upsert(card).On(e => e.CircleCardId).RunAsync();
                await ctx.SaveChangesAsync();


                var cachedClientCards = await _writer.GetAsync(
                    CircleCardNoSqlEntity.GeneratePartitionKey(card.BrokerId),
                    CircleCardNoSqlEntity.GenerateRowKey(card.ClientId));
                if (cachedClientCards != null)
                {
                    var existing = cachedClientCards.Cards.FirstOrDefault(x => x.Id == card.Id);

                    if (existing != null)
                    {
                        cachedClientCards.Cards.Remove(existing);
                    }

                    if (card.IsActive)
                    {
                        cachedClientCards.Cards.Add(card);
                    }
                    await _writer.InsertOrReplaceAsync(cachedClientCards);
                }
                else
                {
                    if (card.IsActive)
                    {
                        var entity = CircleCardNoSqlEntity.Create(card.BrokerId, card.ClientId,
                        new List<CircleCard> { card });
                        await _writer.InsertOrReplaceAsync(entity);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to process SignalCircleCard due to {error}", ex.Message);
            }
        }
    }

}
