using Autofac;
using Service.Circle.Signer.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Circle.Wallets.Client
{
    public static class AutofacHelper
    {
        public static void RegisterCircleWalletsClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new CircleWalletsClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetCircleCardsService()).As<ICircleCardsService>()
                .SingleInstance();
        }
    }
}
