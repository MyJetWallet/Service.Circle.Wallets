using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Domain.Models
{
    [DataContract]
    public class CircleCardPaymentDetails
    {
        [DataMember(Order = 1)] public decimal FeePercentage { get; set; }
        [DataMember(Order = 2)] public decimal MinAmount { get; set; }
        [DataMember(Order = 3)] public decimal MaxAmount { get; set; }
        [DataMember(Order = 4)] public string SettlementAsset { get; set; }
    }
}