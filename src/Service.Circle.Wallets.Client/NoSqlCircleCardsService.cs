using System.Collections.Generic;
using System.Threading.Tasks;
using MyNoSqlServer.DataReader;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Grpc;
using Service.Circle.Wallets.Grpc.Models;

namespace Service.Circle.Wallets.Client
{
    public class NoSqlCircleCardsService : ICircleCardsService
    {
        private readonly ICircleCardsService _grpcService;
        private readonly MyNoSqlReadRepository<CircleCardNoSqlEntity> _reader;

        public NoSqlCircleCardsService(ICircleCardsService grpcService,
            MyNoSqlReadRepository<CircleCardNoSqlEntity> reader)
        {
            _grpcService = grpcService;
            _reader = reader;
        }

        public async Task<Response<CircleCard>> GetCircleClientCard(GetClientCardRequest request)
        {
            if (!request.OnlyActive) return await _grpcService.GetCircleClientCard(request);

            var entity = _reader.Get(CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                CircleCardNoSqlEntity.GenerateRowKey(request.ClientId))?.Cards.Find(e => e.Id == request.CardId && e.IsActive);

            if (entity != null)
                return Response<CircleCard>.Success(entity);

            return await _grpcService.GetCircleClientCard(request);
        }

        public async Task<Response<List<CircleCard>>> GetCircleClientAllCards(GetClientAllCardsRequest request)
        {
            if (!request.OnlyActive) return await _grpcService.GetCircleClientAllCards(request);

            var entity = _reader.Get(CircleCardNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                CircleCardNoSqlEntity.GenerateRowKey(request.ClientId))?.Cards.FindAll(e => e.IsActive);

            if (entity != null)
                return Response<List<CircleCard>>.Success(entity);

            return await _grpcService.GetCircleClientAllCards(request);
        }

        public async Task<Response<CircleCard>> AddCircleCard(AddClientCardRequest request)
        {
            return await _grpcService.AddCircleCard(request);
        }

        public async Task<Response<bool>> DeleteCircleCard(DeleteClientCardRequest request)
        {
            return await _grpcService.DeleteCircleCard(request);
        }

        public Task<Response<bool>> UpdateCardPaymentDetails(CircleCardPaymentDetails request)
        {
            return _grpcService.UpdateCardPaymentDetails(request);
        }

        public Task<Response<CircleCardPaymentDetails>> GetCardPaymentDetails()
        {
            return _grpcService.GetCardPaymentDetails();
        }
    }
}