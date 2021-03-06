using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models.BankAccounts
{
    [DataContract]
    public class GetClientBankAccountRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }
        [DataMember(Order = 3)] public string BankAccountId { get; set; }
        [DataMember(Order = 4)] public bool OnlyActive { get; set; }
    }
}