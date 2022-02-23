using Autofac;
using Service.Circle.Signer.Client;

namespace Service.Circle.Wallets.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterCircleCardsClient(Program.Settings.CircleSignerGrpcServiceUrl);
            builder.RegisterCircleBankAccountsClient(Program.Settings.CircleSignerGrpcServiceUrl);
        }
    }
}