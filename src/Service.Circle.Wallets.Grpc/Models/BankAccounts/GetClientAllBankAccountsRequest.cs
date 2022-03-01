using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models.BankAccounts
{
    [DataContract]
    public class GetClientAllBankAccountsRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }
        [DataMember(Order = 3)] public bool OnlyActive { get; set; }
    }
}