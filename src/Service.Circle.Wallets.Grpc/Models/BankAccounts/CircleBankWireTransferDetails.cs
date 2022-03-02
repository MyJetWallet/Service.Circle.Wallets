using MyJetWallet.Circle.Models.WireTransfers;
using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Grpc.Models.BankAccounts
{
    [DataContract]
    public class CircleBankWireTransferDetails
    {
        [DataMember(Order = 1)]
        public string TrackingRef { get; set; }

        [DataMember(Order = 2)]
        public BankWireTransferDetailBeneficiary Beneficiary { get; set; }

        [DataMember(Order = 3)]
        public BankWireTransferDetailBeneficiaryBank BeneficiaryBank { get; set; }
    }
}