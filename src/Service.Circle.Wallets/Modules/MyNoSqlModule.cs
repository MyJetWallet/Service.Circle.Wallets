using Autofac;
using MyJetWallet.Sdk.NoSql;
using Service.Circle.Wallets.Domain.Models;
using Service.Circle.Wallets.Domain.Models.WireTransfers;

namespace Service.Circle.Wallets.Modules
{
    public class MyNoSqlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<CircleCardNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                CircleCardNoSqlEntity.TableName);

            builder.RegisterMyNoSqlWriter<CircleBankAccountNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                CircleBankAccountNoSqlEntity.TableName);
        }
    }
}