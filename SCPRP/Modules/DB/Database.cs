using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;

namespace SCPRP.Modules.DB
{
    public class DatabaseConfig
    {
        [Description("SQL Server IP address/domain")]
        public string ip { get; set; } = "localhost";
        [Description("SQL Server IP address/domain")]
        public int port { get; set; } = 3306;
        [Description("SQL Server Username")]
        public string user { get; set; } = "root";
        [Description("SQL Server Password")]
        public string pw { get; set; } = "root";
        [Description("SQL Server Database")]
        public string db { get; set; } = "scpsl_rp";

        public string table { get; set; } = "rp_money";
    }


    public class Database : BaseModule<DatabaseConfig>
    {
        public static MySqlConnection DB;
        public static Database Singleton;
        internal static bool ConnectDB()
        {
            if (DB != null && DB.State != System.Data.ConnectionState.Closed) { DB.Close(); }

            var config = Singleton.Config;
            if (config == null) return false;

            DB = new MySqlConnection($"server={config.ip};user={config.user};database={config.db};port={config.port};password={config.pw}");
            DB.Open();

            try
            {
                var cmd = new MySqlCommand($@"CREATE TABLE IF NOT EXISTS `{config.table}` (id VARCHAR(255) PRIMARY KEY, money BIGINT);", DB);
                var affect = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return DB != null && DB.State == System.Data.ConnectionState.Open;
        }
        public override void Load()
        {
            Singleton = this;
            ConnectDB();
        }

        public override void Tick()
        {
            
        }


   

        public override void Unload()
        {
            if (DB != null && DB.State == System.Data.ConnectionState.Open)
                DB.Close();
        }
    }
}
