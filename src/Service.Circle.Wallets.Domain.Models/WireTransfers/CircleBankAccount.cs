using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using MyJetWallet.Circle.Models.WireTransfers;

namespace Service.Circle.Wallets.Domain.Models.WireTransfers
{
    [DataContract]
    public class CircleBankAccount
    {
        public CircleBankAccount(string id, string brokerId, string clientId, 
            string bankAccountId, 
            BankAccountStatus bankAccountStatus,
            string description,
            string trackingRef,
            string fingerPrint,
            string billingDetailsName,
            string billingDetailsCity,
            string billingDetailsCountry,
            string billingDetailsLine1,
            string billingDetailsLine2,
            string billingDetailsDistrict,
            string billingDetailsPostalCode,
            string bankAddressBankName,
            string bankAddressCity,
            string bankAddressCountry,
            string bankAddressLine1,
            string bankAddressLine2,
            string bankAddressDistrict,
            string iban,
            string accountNumber,
            string routingNumber,
            string error, bool isActive, DateTime createDate, DateTime updateDate)
        {
            Id = id;
            BrokerId = brokerId;
            ClientId = clientId;
            BankAccountId = bankAccountId;
            BankAccountStatus = bankAccountStatus;
            Description = description;
            TrackingRef = trackingRef;
            FingerPrint = fingerPrint;
            BillingDetailsName = billingDetailsName;
            BillingDetailsCity = billingDetailsCity;
            BillingDetailsCountry = billingDetailsCountry;
            BillingDetailsLine1 = billingDetailsLine1;
            BillingDetailsLine2 = billingDetailsLine2;
            BillingDetailsDistrict = billingDetailsDistrict;
            BillingDetailsPostalCode = billingDetailsPostalCode;
            BankAddressBankName = bankAddressBankName;
            BankAddressCity = bankAddressCity;
            BankAddressCountry = bankAddressCountry;
            BankAddressLine1 = bankAddressLine1;
            BankAddressLine2 = bankAddressLine2;
            BankAddressDistrict = bankAddressDistrict;
            Error = error;
            IsActive = isActive;
            CreateDate = createDate;
            UpdateDate = updateDate;
            Iban = iban;
            AccountNumber = accountNumber;
            RoutingNumber = routingNumber;
        }

        public CircleBankAccount(CircleBankAccount bank)
        {
            Id = bank.Id;
            BrokerId = bank.BrokerId;
            ClientId = bank.ClientId;
            BankAccountId = bank.BankAccountId;
            BankAccountStatus = bank.BankAccountStatus;
            Description = bank.Description;
            TrackingRef = bank.TrackingRef;
            FingerPrint = bank.FingerPrint;
            BillingDetailsName = bank.BillingDetailsName;
            BillingDetailsCity = bank.BillingDetailsCity;
            BillingDetailsCountry = bank.BillingDetailsCountry;
            BillingDetailsLine1 = bank.BillingDetailsLine1;
            BillingDetailsLine2 = bank.BillingDetailsLine2;
            BillingDetailsDistrict = bank.BillingDetailsDistrict;
            BillingDetailsPostalCode = bank.BillingDetailsPostalCode;
            BankAddressBankName = bank.BankAddressBankName;
            BankAddressCity = bank.BankAddressCity;
            BankAddressCountry = bank.BankAddressCountry;
            BankAddressLine1 = bank.BankAddressLine1;
            BankAddressLine2 = bank.BankAddressLine2;
            BankAddressDistrict = bank.BankAddressDistrict;
            Error = bank.Error;
            IsActive = bank.IsActive;
            CreateDate = bank.CreateDate;
            UpdateDate = bank.UpdateDate;
            Iban = bank.Iban;
            AccountNumber = bank.AccountNumber;
            RoutingNumber = bank.RoutingNumber;
        }

        public CircleBankAccount()
        {
        }

        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string BrokerId { get; set; }
        [DataMember(Order = 3)] public string ClientId { get; set; }
        [DataMember(Order = 4)]public string BankAccountId { get; set; }
        [DataMember(Order = 5)]public BankAccountStatus BankAccountStatus { get; set; }
        [DataMember(Order = 6)]public string Description { get; set; }
        [DataMember(Order = 7)]public string TrackingRef { get; set; }
        [DataMember(Order = 8)]public string FingerPrint { get; set; }
        [DataMember(Order = 9)]public string BillingDetailsName { get; set; }
        [DataMember(Order = 10)]public string BillingDetailsCity { get; set; }
        [DataMember(Order = 11)]public string BillingDetailsCountry { get; set; }
        [DataMember(Order = 12)]public string BillingDetailsLine1 { get; set; }
        [DataMember(Order = 13)]public string BillingDetailsLine2 { get; set; }
        [DataMember(Order = 14)]public string BillingDetailsDistrict { get; set; }
        [DataMember(Order = 15)]public string BillingDetailsPostalCode { get; set; }
        [DataMember(Order = 16)]public string BankAddressBankName { get; set; }
        [DataMember(Order = 17)]public string BankAddressCity { get; set; }
        [DataMember(Order = 18)]public string BankAddressCountry { get; set; }
        [DataMember(Order = 19)]public string BankAddressLine1 { get; set; }
        [DataMember(Order = 20)]public string BankAddressLine2 { get; set; }
        [DataMember(Order = 21)] public string BankAddressDistrict { get; set; }
        [DataMember(Order = 22)] public string Error { get; set; }
        [DataMember(Order = 23)] public bool IsActive { get; set; }
        [DataMember(Order = 24)] public DateTime CreateDate { get; set; }
        [DataMember(Order = 25)] public DateTime UpdateDate { get; set; }
        [DataMember(Order = 26)] public string Iban { get; set; }
        [DataMember(Order = 27)] public string AccountNumber { get; set; }
        [DataMember(Order = 28)] public string RoutingNumber { get; set; }
    }
}