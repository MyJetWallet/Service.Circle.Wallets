using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models.BankAccounts
{
    [DataContract]
    public class GetClientBankAccountByIdRequest
    {
        [DataMember(Order = 1)] public string BankAccountId { get; set; }
    }
}