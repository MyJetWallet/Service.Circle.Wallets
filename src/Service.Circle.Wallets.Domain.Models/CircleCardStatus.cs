// ReSharper disable InconsistentNaming

using System.Runtime.Serialization;

namespace Service.Circle.Wallets.Domain.Models
{
    [DataContract]
    public enum CircleCardStatus
    {
        Pending,
        Complete,
        Failed
    }
}