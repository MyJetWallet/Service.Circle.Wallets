using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace Service.Circle.Wallets.Domain.Models.WireTransfers
{
    public class CircleBankAccountNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-circle-bank-accounts";

        public static string GeneratePartitionKey(string brokerId) => brokerId;
        public static string GenerateRowKey(string clientId) => clientId;

        public List<CircleBankAccount> BankAccounts { get; set; }

        public static CircleBankAccountNoSqlEntity Create(string brokerId, string clientId, List<CircleBankAccount> bankAccounts)
        {
            return new CircleBankAccountNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(brokerId),
                RowKey = GenerateRowKey(clientId),
                BankAccounts = bankAccounts
            };
        }
    }
}