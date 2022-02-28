using MyNoSqlServer.Abstractions;

namespace Service.Circle.Wallets.Domain.Models
{
    public class CircleCardPaymentDetailsNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-circle-cards-payment-details";
        
        public static string GeneratePartitionKey() => "card-payment";
        public static string GenerateRowKey() => "details";
        
        public CircleCardPaymentDetails Details { get; set; }
        
        public static CircleCardPaymentDetailsNoSqlEntity Create(CircleCardPaymentDetails details)
        {
            return new CircleCardPaymentDetailsNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                Details = details
            };
        }
    }
}