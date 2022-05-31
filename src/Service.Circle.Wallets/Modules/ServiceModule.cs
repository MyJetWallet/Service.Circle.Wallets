using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using Service.Circle.Signer.Client;
using Service.Circle.Wallets.Subscribers;
using Service.Circle.Webhooks.Domain.Models;

namespace Service.Circle.Wallets.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterCircleCardsClient(Program.Settings.CircleSignerGrpcServiceUrl);
            builder.RegisterCircleBankAccountsClient(Program.Settings.CircleSignerGrpcServiceUrl);

            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(
                Program.ReloadedSettings(e => e.SpotServiceBusHostPort),
                Program.LogFactory);

            builder.RegisterMyServiceBusSubscriberSingle<SignalCircleCard>(
                serviceBusClient,
                SignalCircleCard.ServiceBusTopicName,
                "service-circle-wallets",
                MyServiceBus.Abstractions.TopicQueueType.Permanent);

            builder
               .RegisterType<CircliCardSignalSubscriber>()
               .SingleInstance()
               .AutoActivate();
        }
    }
}