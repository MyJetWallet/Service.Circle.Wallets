using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models.BankAccounts
{
    [DataContract]
    public class GetCircleBankWireTransferDetailsRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string BankAccountId { get; set; }
    }
}