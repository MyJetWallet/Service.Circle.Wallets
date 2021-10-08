using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.GrpcMetrics;
using MyNoSqlServer.DataReader;
using ProtoBuf.Grpc.Client;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Grpc;

namespace Service.Circle.Wallets.Client
{
    [UsedImplicitly]
    public class CircleWalletsClientFactory : MyGrpcClientFactory
    {
        private readonly CallInvoker _channel;
        private readonly MyNoSqlReadRepository<CircleCardNoSqlEntity> _reader;

        public CircleWalletsClientFactory(string grpcServiceUrl, MyNoSqlReadRepository<CircleCardNoSqlEntity> reader) :
            base(grpcServiceUrl)
        {
            _reader = reader;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(grpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public ICircleCardsService GetCircleCardsService() =>
            _reader != null
                ? new NoSqlCircleCardsService(_channel.CreateGrpcService<ICircleCardsService>(), _reader)
                : _channel.CreateGrpcService<ICircleCardsService>();
    }
}