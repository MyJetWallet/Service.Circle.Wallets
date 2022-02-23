using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.Circle.Wallets.Domain.Models.WireTransfers;
using Service.Circle.Wallets.Grpc.Models;
using Service.Circle.Wallets.Grpc.Models.BankAccounts;

namespace Service.Circle.Wallets.Grpc
{
    [ServiceContract]
    public interface ICircleBankAccountsService
    {
        [OperationContract]
        Task<Response<CircleBankAccount>> GetCircleBankAccount(GetClientBankAccountRequest request);

        [OperationContract]
        Task<Response<List<CircleBankAccount>>> GetCircleClientAllBankAccounts(GetClientAllBankAccountsRequest request);

        [OperationContract]
        Task<Response<CircleBankAccount>> AddCircleBankAccount(AddClientBankAccountRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteCircleBankAccount(DeleteClientBankAccountRequest request);
    }
}