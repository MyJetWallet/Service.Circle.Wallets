using MyJetWallet.Circle.Models.WireTransfers;
using System;
using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models.BankAccounts
{
    [DataContract]
    public class AddClientBankAccountRequest
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string BrokerId { get; set; }
        [DataMember(Order = 3)] public string ClientId { get; set; }
        [DataMember(Order = 4)] public string AccountNumber { get; set; }
        [DataMember(Order = 5)] public string RoutingNumber { get; set; }
        [DataMember(Order = 6)] public string Iban { get; set; }
        [DataMember(Order = 9)] public string BillingDetailsName { get; set; }
        [DataMember(Order = 10)] public string BillingDetailsCity { get; set; }
        [DataMember(Order = 11)] public string BillingDetailsCountry { get; set; }
        [DataMember(Order = 12)] public string BillingDetailsLine1 { get; set; }
        [DataMember(Order = 13)] public string BillingDetailsLine2 { get; set; }
        [DataMember(Order = 14)] public string BillingDetailsDistrict { get; set; }
        [DataMember(Order = 15)] public string BillingDetailsPostalCode { get; set; }
        [DataMember(Order = 16)] public string BankAddressBankName { get; set; }
        [DataMember(Order = 17)] public string BankAddressCity { get; set; }
        [DataMember(Order = 18)] public string BankAddressCountry { get; set; }
        [DataMember(Order = 19)] public string BankAddressLine1 { get; set; }
        [DataMember(Order = 20)] public string BankAddressLine2 { get; set; }
        [DataMember(Order = 21)] public string BankAddressDistrict { get; set; }
    }
}