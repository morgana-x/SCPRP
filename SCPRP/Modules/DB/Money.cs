using LabApi.Features.Wrappers;
using MySql.Data.MySqlClient;
using System;

namespace SCPRP.Modules.DB
{

    public class MoneyConfig
    {
        public int StartingMoney { get; set; } = 20000;
    }

    public class Money : BaseModule<MoneyConfig>
    {
        public static Money Singleton;
        public override void Load()
        {
            Singleton = this;
        }

        public override void Unload()
        {
            
        }
        public static long GetMoney(string userid)
        {
            var DB = Database.DB;
            if (DB == null || DB.State != System.Data.ConnectionState.Open)
            {
                if (!Database.ConnectDB())
                    return 0;
            }
            try
            {
                var cmd = new MySqlCommand($@"SELECT money FROM {Database.Singleton.Config.table} WHERE id='{userid}';", DB);
                var rd = cmd.ExecuteReader();
                long value = 0;
                try
                {
                   
                    if (rd.Read())
                    {
                        value = rd.GetInt64(0);
                    }
                    else // If there's no entry for the player, set their money to the starting amount
                    {
                        rd.Close();
                        SetMoney(userid, Singleton.Config.StartingMoney);
                        value = Singleton.Config.StartingMoney;
                    }
                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
   
                rd.Close();
                cmd.Dispose();
                return value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        public static void SetMoney(string userid, long amount)
        {
            if (Database.DB == null || Database.DB.State != System.Data.ConnectionState.Open)
            {
                return;
            }

            var cmd = new MySqlCommand($@"REPLACE INTO {Database.Singleton.Config.table} (id, money) VALUES ('{userid}', {amount});", Database.DB);
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


    }
}
