using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Domain.Models
{
    [DataContract]
    public class CircleCardPaymentDetails
    {
        // percentage part fee for the transaction
        [DataMember(Order = 1)] public decimal FeePercentage { get; set; }
        [DataMember(Order = 2)] public decimal MinAmount { get; set; }
        [DataMember(Order = 3)] public decimal MaxAmount { get; set; }
        [DataMember(Order = 4)] public string SettlementAsset { get; set; }
        // fix part in circle fee for the transaction
        [DataMember(Order = 5)] public decimal FixFeeAmount { get; set; }
        [DataMember(Order = 6)] public decimal Day1Limit { get; set; }
        [DataMember(Order = 7)] public decimal Day7Limit { get; set; }
        [DataMember(Order = 8)] public decimal Day30Limit { get; set; }

    }
}