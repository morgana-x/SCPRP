using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using LabApi.Features.Wrappers;

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

    public class MoneyConfig
    {
        public int StartingMoney { get; set; } = 20000;
    }

    public class Database : BaseModule
    {
        public static MySqlConnection DB;

        public static DatabaseConfig Config { get { return SCPRP.Singleton.Config.DatabaseConfig;  } }

        public override void Load()
        {
            if (DB != null && DB.State != System.Data.ConnectionState.Closed) { DB.Close(); }
            DB = new MySql.Data.MySqlClient.MySqlConnection($"server={Config.ip};user={Config.user};database={Config.db};port={Config.port};password={Config.pw}");
            DB.Open();

            var cmd = new MySqlCommand($@"CREATE TABLE IF NOT EXISTS `{Config.table}` (id VARCHAR(255) PRIMARY KEY, money BIGINT);", DB);
            var affect = cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public override void Tick()
        {
            
        }

        public static long GetMoney(string userid)
        {
            if (DB == null || DB.State != System.Data.ConnectionState.Open)
                return 0;

            var cmd = new MySqlCommand($@"SELECT money FROM {Config.table} WHERE id='{userid}';", DB);
            var rd = cmd.ExecuteReader();

            long value = 0;
            if (rd.Read())
            {
                value = rd.GetInt64(0);
            }
            else // If there's no entry for the player, set their money to the starting amount
            {
                rd.Close();
                SetMoney(userid, SCPRP.Singleton.Config.MoneyConfig.StartingMoney);
                value = SCPRP.Singleton.Config.MoneyConfig.StartingMoney;
            }
            rd.Close();
            cmd.Dispose();
            return value;
        }

        public static void SetMoney(string userid, long amount)
        {
            if (DB == null || DB.State != System.Data.ConnectionState.Open) { return; }

            var cmd = new MySqlCommand($@"REPLACE INTO {Config.table} (id, money) VALUES ('{userid}', {amount});", DB);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public static void AddMoney(string userid, long amount)
        {
            SetMoney(userid, Math.Min(Math.Max(GetMoney(userid) + amount, 0), long.MaxValue - 1));
        }

        public static void AddMoney(Player pl, long amount) => AddMoney(pl.UserId, amount);
        public static void SetMoney(Player pl, long amount) => SetMoney(pl.UserId, amount);
        public static long GetMoney(Player pl) => GetMoney(pl.UserId);

        public override void Unload()
        {
            if (DB != null && DB.State == System.Data.ConnectionState.Open)
                DB.Close();
        }
    }
}
