using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Grpc.Models;

namespace Service.Circle.Wallets.Grpc
{
    [ServiceContract]
    public interface ICircleCardsService
    {
        [OperationContract]
        Task<Response<CircleCard>> GetCircleClientCard(GetClientCardRequest request);

        [OperationContract]
        Task<Response<List<CircleCard>>> GetCircleClientAllCards(GetClientAllCardsRequest request);

        [OperationContract]
        Task<Response<CircleCard>> AddCircleCard(AddClientCardRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteCircleCard(DeleteClientCardRequest request);
    }
}