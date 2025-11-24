using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
namespace SCPRP.Modules.Players.Jobs
{
    public class HitmanConfig
    {
        public int MinimumPrice { get; set; } = 2000;
    }

    public class Hitman : BaseModule<HitmanConfig>
    {
        public static Hitman Singleton;

        public Dictionary<string, int> Hits = new Dictionary<string, int>();
        public override void Load()
        {
            Singleton = this;
            PlayerEvents.Death += Death;
        }

        public override void Unload()
        {
            PlayerEvents.Death -= Death;
        }

        public static void AddHit(Player player, int amount)
        {
            if (!Singleton.Hits.ContainsKey(player.UserId))
                Singleton.Hits.Add(player.UserId, amount);
            else
                Singleton.Hits[player.UserId] = Singleton.Hits[player.UserId] + amount;
        }

        public static void RemoveHit(Player player)
        {
            if (Singleton.Hits.ContainsKey(player.UserId))
                Singleton.Hits.Remove(player.UserId);
        }

        public static int GetHit(Player player)
        {
            return ((Singleton != null) && Singleton.Hits.ContainsKey(player.UserId)) ? Singleton.Hits[player.UserId] : 0;
        }

        public static bool IsHitman(Player player)
        {
            return player.GetJobInfo().Hitman;
        }
        public override void Tick()
        {
            
        }

        void Death(PlayerDeathEventArgs e)
        {
            if (GetHit(e.Player) > 0 && e.Attacker != null && IsHitman(e.Attacker))
            {
                var hitamount = GetHit(e.Player);
                e.Attacker.AddMoney(hitamount);
                RemoveHit(e.Player);

                e.Attacker.Notify($"Collected {e.Player.DisplayName}'s ${hitamount} bounty!");
                e.Player.Notify($"You were killed by hitmen for your ${hitamount} bounty!");
            }
        }



    }
}
