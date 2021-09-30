using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.Circle.Wallets.Grpc;

namespace Service.Circle.Wallets.Client
{
    [UsedImplicitly]
    public class CircleWalletsClientFactory : MyGrpcClientFactory
    {
        private readonly CallInvoker _channel;

        public CircleWalletsClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(grpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public ICircleCardsService GetCircleCardsService() =>
            _channel.CreateGrpcService<ICircleCardsService>();
    }
}