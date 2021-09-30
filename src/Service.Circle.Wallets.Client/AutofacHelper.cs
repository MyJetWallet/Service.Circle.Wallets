using Autofac;

// ReSharper disable UnusedMember.Global

namespace Service.Circle.Wallets.Client
{
    public static class AutofacHelper
    {
        public static void RegisterCircleWalletsClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new CircleWalletsClientFactory(grpcServiceUrl);
        }
    }
}
