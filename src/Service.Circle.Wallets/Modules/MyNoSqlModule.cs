using Autofac;
using MyJetWallet.Sdk.NoSql;
using Service.Circle.Wallets.Domain.Models;

namespace Service.Circle.Wallets.Modules
{
    public class MyNoSqlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<CircleCardNoSqlEntity>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                CircleCardNoSqlEntity.TableName);
        }
    }
}