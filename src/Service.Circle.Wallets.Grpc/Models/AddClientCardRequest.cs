using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models
{
    [DataContract]
    public class AddClientCardRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }
        [DataMember(Order = 3)] public string IdempotencyKey { get; set; }
        [DataMember(Order = 4)] public string KeyId { get; set; }
        [DataMember(Order = 5)] public string EncryptedData { get; set; }
        [DataMember(Order = 6)] public string BillingName { get; set; }
        [DataMember(Order = 7)] public string BillingCity { get; set; }
        [DataMember(Order = 8)] public string BillingCountry { get; set; }
        [DataMember(Order = 9)] public string BillingLine1 { get; set; }
        [DataMember(Order = 10)] public string BillingLine2 { get; set; }
        [DataMember(Order = 11)] public string BillingDistrict { get; set; }
        [DataMember(Order = 12)] public string BillingPostalCode { get; set; }
        [DataMember(Order = 13)] public int ExpMonth { get; set; }
        [DataMember(Order = 14)] public int ExpYear { get; set; }
        [DataMember(Order = 15)] public string SessionId { get; set; }
        [DataMember(Order = 16)] public string IpAddress { get; set; }
        [DataMember(Order = 17)] public string CardName { get; set; }
    }
}