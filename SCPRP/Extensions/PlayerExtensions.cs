using LabApi.Features.Wrappers;
using SCPRP.Modules.DB;
namespace SCPRP.Extensions
{
    public static class PlayerExtensions
    {
        public static long GetMoney(this Player player)
        {
            return Database.GetMoney(player);
        }

        public static void SetMoney(this Player player, long amount)
        {
            Database.SetMoney(player, amount);
        }

        public static void AddMoney(this Player player, long amount)
        {
            Database.AddMoney(player, amount);
        }
    }
}
