using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models
{
    [DataContract]
    public class UpdateClientCardRequest
    {  
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }      
        [DataMember(Order = 3)] public long CardId { get; set; }
        [DataMember(Order = 4)] public string KeyId { get; set; }
        [DataMember(Order = 5)] public string EncryptedData { get; set; }
        [DataMember(Order = 6)] public int ExpMonth { get; set; }
        [DataMember(Order = 7)] public int ExpYear { get; set; }
    }
}