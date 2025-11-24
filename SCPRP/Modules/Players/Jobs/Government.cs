using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;

namespace SCPRP.Modules.Players.Jobs
{
    public class GovernmentConfig
    {
        public int WarrantExpirySeconds { get; set; } = 350;
        public int WantedExpirySeconds { get; set; } = 350;
    }
    public class Government : BaseModule<GovernmentConfig>
    {

        public class WantedStatus
        {
            public Player WantedPlayer { get; set; }
            public Player WarrantingOfficer { get; set; }
            public string Reason { get; set; }
            public DateTime IssueDate { get; set; }

            public virtual bool Expired()
            {
                return DateTime.Now.Subtract(IssueDate).TotalSeconds > Singleton.Config.WantedExpirySeconds;
            }

            public WantedStatus(Player player, Player officer, string reason)
            {
                this.WantedPlayer = player;
                this.WarrantingOfficer = officer;
                this.Reason = reason;
                IssueDate = DateTime.Now;
            }
        }

        public class WarrantedStatus : WantedStatus
        {
            public WarrantedStatus(Player player, Player officer, string reason) : base(player, officer, reason) { }
            public override bool Expired()
            {
                return DateTime.Now.Subtract(IssueDate).TotalSeconds > Singleton.Config.WarrantExpirySeconds;
            }
        }


        public Dictionary<string, WantedStatus> Wanted      = new Dictionary<string, WantedStatus>();
        public Dictionary<string, WarrantedStatus> Warranted   = new Dictionary<string, WarrantedStatus>();

        public static Government Singleton;


        public static bool IsGovernment(Player player)
        {
            return player.GetJobInfo().Team == "government";
        }

        public static bool IsWanted(Player player)
        {
            return (Singleton != null) && Singleton.Wanted.ContainsKey(player.UserId);
        }
        public static bool IsWarranted(Player player)
        {
            return (Singleton != null) && Singleton.Warranted.ContainsKey(player.UserId);
        }

        public static void SetWanted(Player player, string reason, Player officer)
        {
            var wantedstatus = new WantedStatus(player, officer, reason);
            if (Singleton.Wanted.ContainsKey(player.UserId))
                Singleton.Wanted[player.UserId] = wantedstatus;
            else
                Singleton.Wanted.Add(player.UserId, wantedstatus);
        }

        public static void Unwant(Player player)
        {
            if (Singleton.Wanted.ContainsKey(player.UserId))
                Singleton.Wanted.Remove(player.UserId);
        }
        public static void SetWarranted(Player player, string reason, Player officer)
        {
            var wantedstatus = new WarrantedStatus(player, officer, reason);
            if (Singleton.Warranted.ContainsKey(player.UserId))
                Singleton.Warranted[player.UserId] = wantedstatus;
            else
                Singleton.Warranted.Add(player.UserId, wantedstatus);
        }

        public override void Load()
        {
            Singleton = this;
            PlayerEvents.Cuffing += Cuffing;
        }

        public override void Unload()
        {
            PlayerEvents.Cuffing -= Cuffing;
        }
        void Cuffing(PlayerCuffingEventArgs e)
        {
            if (!IsGovernment(e.Player))
                e.IsAllowed = false;
        }

        public override void Tick()
        {
            foreach(var item in Wanted.Keys.ToList())
            {
                if (Wanted[item].Expired())
                {
                    Wanted.Remove(item);
                }
            }
            foreach (var item in Warranted.Keys.ToList())
            {
                if (Warranted[item].Expired())
                {
                    Warranted.Remove(item);
                }
            }
        }

    }
}
