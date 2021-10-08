using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Circle.Wallets.Settings
{
    public class SettingsModel
    {
        [YamlProperty("CircleWallets.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("CircleWallets.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("CircleWallets.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("CircleWallets.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("CircleWallets.CircleSignerGrpcServiceUrl")]
        public string CircleSignerGrpcServiceUrl { get; set; }

        [YamlProperty("CircleWallets.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("CircleWallets.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }
    }
}