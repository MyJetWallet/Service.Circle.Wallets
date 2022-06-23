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
                _logger.LogInformation("Processing SignalCircleCard {card}", cardSignal.ToJson());

                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                var card = await ctx.Cards.FirstOrDefaultAsync(x => x.CircleCardId == cardSignal.CircleCardId);

                if (card == null)
                {
                    _logger.LogError("Processing SignalCircleCard does not exist. {error}", cardSignal.ToJson());
                    return;
                }

                card.UpdateDate = cardSignal.UpdateDate;
                card.Bin = cardSignal.Bin;
                card.FingerPrint = cardSignal.Fingerprint;
                card.RiskEvaluation = cardSignal.RiskEvaluation;
                card.FundingType = cardSignal.FundingType;
                card.IssuerCountry = cardSignal.IssuerCountry;

                switch (cardSignal.Status)
                {
                    case MyJetWallet.Circle.Models.Cards.CardStatus.Pending:
                        card.Status = CircleCardStatus.Pending;
                        break;
                    case MyJetWallet.Circle.Models.Cards.CardStatus.Complete:
                        card.Status = CircleCardStatus.Complete;
                        break;
                    case MyJetWallet.Circle.Models.Cards.CardStatus.Failed:
                        card.Status = CircleCardStatus.Failed;
                        card.IsActive = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(cardSignal.Status), cardSignal.Status, null);
                }

                await ctx.Cards.Upsert(card)
                    .On(e => e.CircleCardId)
                    .UpdateIf(e => e.UpdateDate <= card.UpdateDate)
                    .RunAsync();
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
