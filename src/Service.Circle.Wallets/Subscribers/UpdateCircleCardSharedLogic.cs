using Microsoft.EntityFrameworkCore;
using MyNoSqlServer.Abstractions;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Postgres;
using Service.Circle.Wallets.Postgres.Models;
using Service.Circle.Webhooks.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Circle.Wallets.Subscribers
{
    public class UpdateCircleCardSharedLogic
    {
        public async Task<CircleCardEntity> ExecuteAsync(DatabaseContext ctx,
            IMyNoSqlServerDataWriter<CircleCardNoSqlEntity> writer, SignalCircleCard cardSignal, CircleCardEntity card)
        {
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


            var cachedClientCards = await writer.GetAsync(
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
                await writer.InsertOrReplaceAsync(cachedClientCards);
            }
            else
            {
                if (card.IsActive)
                {
                    var entity = CircleCardNoSqlEntity.Create(card.BrokerId, card.ClientId,
                    new List<CircleCard> { card });
                    await writer.InsertOrReplaceAsync(entity);
                }
            }

            return card;
        }
    }

}
