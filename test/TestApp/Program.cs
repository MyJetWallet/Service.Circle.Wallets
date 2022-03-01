using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.Circle.Wallets.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var fac = new CircleWalletsClientFactory("http://localhost:5001", null,null);
            var client = fac.GetCircleBankAccountsService();

            var x =await client.GetCircleBankAccount(new ()
            {
                BankAccountId = "230c9646-53d5-4df2-9fe9-dc7131cadcb9",
                BrokerId = "jetwallet",
                ClientId = "c4484e1f65494e6f948defe7582517ad",
                OnlyActive = true
            });

            var z = await client.GetCircleClientAllBankAccounts(new()
            {
                BrokerId = "jetwallet",
                ClientId = "c4484e1f65494e6f948defe7582517ad",
                OnlyActive = true
            });

            var acc = await client.AddCircleBankAccount(new Service.Circle.Wallets.Grpc.Models.BankAccounts.AddClientBankAccountRequest
            {
                AccountNumber = "123456789",
                //BankAddressBankName = "",
                //BankAddressCity = ,
                BankAddressCountry = "US",
                //BankAddressDistrict= ,
                //BankAddressLine1 = ,
                //BankAddressLine2 = ,
                BillingDetailsCity = "Boston",
                BillingDetailsCountry = "US",
                BillingDetailsDistrict = "MA",
                BillingDetailsLine1 = "1 Main Street",
                //illingDetailsLine2 = //,
                BillingDetailsName = "John Smith",
                BillingDetailsPostalCode = "02201",
                BrokerId = "jetwallet",
                ClientId = "c4484e1f65494e6f948defe7582517ad",
                Iban = null,
                Id = "6ae62bf2-bd71-49ce-a599-165ffcc33680",
                RoutingNumber = "021000021",
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}