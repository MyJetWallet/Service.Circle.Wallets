using System.Collections.Generic;
using System.Threading.Tasks;
using MyNoSqlServer.DataReader;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Domain.Models.WireTransfers;
using Service.Circle.Wallets.Grpc;
using Service.Circle.Wallets.Grpc.Models;
using Service.Circle.Wallets.Grpc.Models.BankAccounts;

namespace Service.Circle.Wallets.Client
{
    public class NoSqlCircleBankAccountsService : ICircleBankAccountsService
    {
        private readonly ICircleBankAccountsService _grpcService;
        private readonly MyNoSqlReadRepository<CircleBankAccountNoSqlEntity> _reader;

        public NoSqlCircleBankAccountsService(ICircleBankAccountsService grpcService,
            MyNoSqlReadRepository<CircleBankAccountNoSqlEntity> reader)
        {
            _grpcService = grpcService;
            _reader = reader;
        }

        public async Task<Response<CircleBankAccount>> GetCircleBankAccount(GetClientBankAccountRequest request)
        {
            if (!request.OnlyActive) return await _grpcService.GetCircleBankAccount(request);

            var entity = _reader.Get(CircleBankAccountNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                CircleBankAccountNoSqlEntity.GenerateRowKey(request.ClientId))?.BankAccounts.Find(e => e.BankAccountId == request.BankAccountId && e.IsActive);

            if (entity != null)
                return Response<CircleBankAccount>.Success(entity);

            return await _grpcService.GetCircleBankAccount(request);
        }

        public async Task<Response<List<CircleBankAccount>>> GetCircleClientAllBankAccounts(GetClientAllBankAccountsRequest request)
        {
            if (!request.OnlyActive) return await _grpcService.GetCircleClientAllBankAccounts(request);

            var entity = _reader.Get(CircleBankAccountNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                CircleBankAccountNoSqlEntity.GenerateRowKey(request.ClientId))?.BankAccounts.FindAll(e => e.IsActive);

            if (entity != null)
                return Response<List<CircleBankAccount>>.Success(entity);

            return await _grpcService.GetCircleClientAllBankAccounts(request);
        }

        public Task<Response<CircleBankAccount>> AddCircleBankAccount(AddClientBankAccountRequest request)
        {
            return _grpcService.AddCircleBankAccount(request);
        }

        public Task<Response<bool>> DeleteCircleBankAccount(DeleteClientBankAccountRequest request)
        {
            return _grpcService.DeleteCircleBankAccount(request);
        }

        public Task<Response<CircleBankWireTransferDetails>> GetCircleBankWireTransferDetails(GetCircleBankWireTransferDetailsRequest request)
        {
            return _grpcService.GetCircleBankWireTransferDetails(request);
        }
    }
}