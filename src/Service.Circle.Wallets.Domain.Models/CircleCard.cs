using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using MyJetWallet.Circle.Models;
using MyJetWallet.Circle.Models.Cards;

namespace Service.Circle.Wallets.Domain.Models
{
    [DataContract]
    public class CircleCard
    {
        public CircleCard(string id, string brokerId, string clientId, string cardName, [CanBeNull] string circleCardId,
            [CanBeNull] string last4, [CanBeNull] string network, int? expMonth, int? expYear, CircleCardStatus status,
            string? error, bool isActive, DateTime createDate, DateTime updateDate,
            string bin, string fingerPrint, RiskEvaluation riskEvaluation, CardFundingType fundingType, string issuerCountry, CardVerificationError? errorCode)
        {
            Id = id;
            BrokerId = brokerId;
            ClientId = clientId;
            CardName = cardName;
            CircleCardId = circleCardId;
            Last4 = last4;
            Network = network;
            ExpMonth = expMonth;
            ExpYear = expYear;
            Status = status;
            Error = error;
            IsActive = isActive;
            CreateDate = createDate;
            UpdateDate = updateDate;
            Bin = bin;
            FingerPrint = fingerPrint;
            RiskEvaluation = riskEvaluation;
            FundingType = fundingType;
            IssuerCountry = issuerCountry;
            ErrorCode = errorCode;
        }

        public CircleCard(CircleCard card)
        {
            Id = card.Id;
            BrokerId = card.BrokerId;
            ClientId = card.ClientId;
            CardName = card.CardName;
            CircleCardId = card.CircleCardId;
            Last4 = card.Last4;
            Network = card.Network;
            ExpMonth = card.ExpMonth;
            ExpYear = card.ExpYear;
            Status = card.Status;
            Error = card.Error;
            IsActive = card.IsActive;
            CreateDate = card.CreateDate;
            UpdateDate = card.UpdateDate;
            Bin = card.Bin;
            FingerPrint = card.FingerPrint;
            RiskEvaluation = card.RiskEvaluation;
            FundingType = card.FundingType;
            IssuerCountry = card.IssuerCountry;
            ErrorCode = card.ErrorCode;
        }

        public CircleCard()
        {
        }

        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string BrokerId { get; set; }
        [DataMember(Order = 3)] public string ClientId { get; set; }
        [DataMember(Order = 4)] public string CardName { get; set; }
        [DataMember(Order = 5)][CanBeNull] public string CircleCardId { get; set; }
        [DataMember(Order = 6)][CanBeNull] public string Last4 { get; set; }
        [DataMember(Order = 7)][CanBeNull] public string Network { get; set; }
        [DataMember(Order = 8)] public int? ExpMonth { get; set; }
        [DataMember(Order = 9)] public int? ExpYear { get; set; }
        [DataMember(Order = 10)] public CircleCardStatus Status { get; set; }
        [DataMember(Order = 11)] public string Error { get; set; }
        [DataMember(Order = 12)] public bool IsActive { get; set; }
        [DataMember(Order = 13)] public DateTime CreateDate { get; set; }
        [DataMember(Order = 14)] public DateTime UpdateDate { get; set; }
        [DataMember(Order = 15)] public string Bin { get; set; }
        [DataMember(Order = 16)] public string FingerPrint { get; set; }
        [DataMember(Order = 17)] public RiskEvaluation RiskEvaluation { get; set; }
        [DataMember(Order = 18)] public CardFundingType FundingType { get; set; }
        [DataMember(Order = 19)] public string IssuerCountry { get; set; }
        [DataMember(Order = 20)] public CardVerificationError? ErrorCode { get; set; }
    }
}