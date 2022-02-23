using Autofac;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Domain.Models.WireTransfers;
using Service.Circle.Wallets.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Circle.Wallets.Client
{
    public static class AutofacHelper
    {
        /// <summary>
        /// Register interfaces:
        ///   * IClientWalletService
        /// </summary>
        public static void RegisterCircleWalletsClient(this ContainerBuilder builder,
            IMyNoSqlSubscriber myNoSqlSubscriber, string grpcServiceUrl)
        {
            var subs = new MyNoSqlReadRepository<CircleCardNoSqlEntity>(myNoSqlSubscriber,
                CircleCardNoSqlEntity.TableName);

            var subBanks = new MyNoSqlReadRepository<CircleBankAccountNoSqlEntity>(myNoSqlSubscriber,
                CircleBankAccountNoSqlEntity.TableName);

            var factory = new CircleWalletsClientFactory(grpcServiceUrl, subs, subBanks);

            builder
                .RegisterInstance(subs)
                .As<IMyNoSqlServerDataReader<CircleCardNoSqlEntity>>()
                .SingleInstance();

            builder
               .RegisterInstance(subBanks)
               .As<IMyNoSqlServerDataReader<CircleBankAccountNoSqlEntity>>()
               .SingleInstance();

            builder
                .RegisterInstance(factory.GetCircleCardsService())
                .As<ICircleCardsService>()
                .AutoActivate()
                .SingleInstance();

            builder
                .RegisterInstance(factory.GetCircleBankAccountsService())
                .As<ICircleBankAccountsService>()
                .AutoActivate()
                .SingleInstance();
        }

        /// <summary>
        /// Register interfaces:
        ///   * IClientWalletService
        /// </summary>
        public static void RegisterCircleWalletsClientWithoutCache(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new CircleWalletsClientFactory(grpcServiceUrl, null, null);

            builder
                .RegisterInstance(factory.GetCircleCardsService())
                .As<ICircleCardsService>()
                .AutoActivate()
                .SingleInstance();

            builder
               .RegisterInstance(factory.GetCircleBankAccountsService())
               .As<ICircleBankAccountsService>()
               .AutoActivate()
               .SingleInstance();
        }
    }
}