using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace Service.Circle.Wallets.Domain.Models
{
    public class CircleCardNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-circle-cards";

        public static string GeneratePartitionKey(string brokerId) => brokerId;
        public static string GenerateRowKey(string clientId) => clientId;

        public List<CircleCard> Cards { get; set; }

        public static CircleCardNoSqlEntity Create(string brokerId, string clientId, List<CircleCard> cards)
        {
            return new CircleCardNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(brokerId),
                RowKey = GenerateRowKey(clientId),
                Cards = cards
            };
        }
    }
}