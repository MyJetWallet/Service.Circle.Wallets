using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Postgres;
using Service.Circle.Webhooks.Domain.Models;
using System;
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

                var logic = new UpdateCircleCardSharedLogic();
                await logic.ExecuteAsync(ctx, _writer, cardSignal, card);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to process SignalCircleCard due to {error}", ex.Message);
            }
        }
    }

}
