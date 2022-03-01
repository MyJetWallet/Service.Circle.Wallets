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
                ClientId = "111",
                Iban = null,
                Id = "6ae62bf2-bd71-49ce-a599-165ffcc33680",
                RoutingNumber = "021000021",
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}