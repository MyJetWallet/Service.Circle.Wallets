using System.Runtime.Serialization;
using MyJetWallet.Circle.Models.Cards;

namespace Service.Circle.Wallets.Domain.Models
{
    [DataContract]
    public class CircleCard
    {
        public CircleCard(long id, string brokerId, string clientId, string circleCardId, string billingName,
            string billingCity, string billingCountry, string billingLine1, string billingLine2, string billingDistrict,
            string billingPostalCode, int expMonth, int expYear, string email, string phoneNumber, string sessionId,
            string ipAddress, CircleCardStatus status, string network, string last4, string bin, string issuerCountry,
            CardFundingType fundingType, string fingerprint, CardVerificationError? errorCode, string createDate,
            string updateDate)
        {
            Id = id;
            BrokerId = brokerId;
            ClientId = clientId;
            CircleCardId = circleCardId;
            BillingName = billingName;
            BillingCity = billingCity;
            BillingCountry = billingCountry;
            BillingLine1 = billingLine1;
            BillingLine2 = billingLine2;
            BillingDistrict = billingDistrict;
            BillingPostalCode = billingPostalCode;
            ExpMonth = expMonth;
            ExpYear = expYear;
            Email = email;
            PhoneNumber = phoneNumber;
            SessionId = sessionId;
            IpAddress = ipAddress;
            Status = status;
            Network = network;
            Last4 = last4;
            Bin = bin;
            IssuerCountry = issuerCountry;
            FundingType = fundingType;
            Fingerprint = fingerprint;
            ErrorCode = errorCode;
            CreateDate = createDate;
            UpdateDate = updateDate;
        }

        public CircleCard(CircleCard card)
        {
            Id = card.Id;
            BrokerId = card.BrokerId;
            ClientId = card.ClientId;
            CircleCardId = card.CircleCardId;
            BillingName = card.BillingName;
            BillingCity = card.BillingCity;
            BillingCountry = card.BillingCountry;
            BillingLine1 = card.BillingLine1;
            BillingLine2 = card.BillingLine2;
            BillingDistrict = card.BillingDistrict;
            BillingPostalCode = card.BillingPostalCode;
            ExpMonth = card.ExpMonth;
            ExpYear = card.ExpYear;
            Email = card.Email;
            PhoneNumber = card.PhoneNumber;
            SessionId = card.SessionId;
            IpAddress = card.IpAddress;
            Status = card.Status;
            Network = card.Network;
            Last4 = card.Last4;
            Bin = card.Bin;
            IssuerCountry = card.IssuerCountry;
            FundingType = card.FundingType;
            Fingerprint = card.Fingerprint;
            ErrorCode = card.ErrorCode;
            CreateDate = card.CreateDate;
            UpdateDate = card.UpdateDate;
        }

        public CircleCard()
        {
        }

        [DataMember(Order = 1)] public long Id { get; internal set; }
        [DataMember(Order = 2)] public string BrokerId { get; set; }
        [DataMember(Order = 3)] public string ClientId { get; set; }
        [DataMember(Order = 4)] public string CircleCardId { get; set; }
        [DataMember(Order = 5)] public string BillingName { get; set; }
        [DataMember(Order = 6)] public string BillingCity { get; set; }
        [DataMember(Order = 7)] public string BillingCountry { get; set; }
        [DataMember(Order = 8)] public string BillingLine1 { get; set; }
        [DataMember(Order = 9)] public string BillingLine2 { get; set; }
        [DataMember(Order = 10)] public string BillingDistrict { get; set; }
        [DataMember(Order = 11)] public string BillingPostalCode { get; set; }
        [DataMember(Order = 12)] public int ExpMonth { get; set; }
        [DataMember(Order = 13)] public int ExpYear { get; set; }
        [DataMember(Order = 14)] public string Email { get; set; }
        [DataMember(Order = 15)] public string PhoneNumber { get; set; }
        [DataMember(Order = 16)] public string SessionId { get; set; }
        [DataMember(Order = 17)] public string IpAddress { get; set; }
        [DataMember(Order = 18)] public CircleCardStatus Status { get; set; }
        [DataMember(Order = 19)] public string Network { get; set; }
        [DataMember(Order = 20)] public string Last4 { get; set; }
        [DataMember(Order = 21)] public string Bin { get; set; }
        [DataMember(Order = 22)] public string IssuerCountry { get; set; }
        [DataMember(Order = 23)] public CardFundingType FundingType { get; set; }
        [DataMember(Order = 24)] public string Fingerprint { get; set; }
        [DataMember(Order = 25)] public CardVerificationError? ErrorCode { get; set; }
        [DataMember(Order = 26)] public string CreateDate { get; set; }
        [DataMember(Order = 27)] public string UpdateDate { get; set; }
    }
}