using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models
{
    [DataContract]
    public class GetCardByCircleIdRequest
    {
        [DataMember(Order = 1)] public string CircleCardId { get; set; }
    }
    
}