using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MonoMod.Utils;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Extensions;
using LabApi.Features.Extensions;
using SCPRP.Extensions;

namespace SCPRP.Modules.Players
{
    public class JobDefinition
    {
        
        public string Name { get; set; }  = "Rolename";
        public string Description { get; set; }  = "A role";
        public string Colour { get; set; } = "#ffffff";

        public RoleTypeId Model { get; set; } = RoleTypeId.ClassD;

        public RoleTypeId Spawnpoint { get; set; }  = RoleTypeId.None;

        public int Payday { get; set; } = 0;
        public int MaxPlayers { get; set; } = 0;
        public string Team { get; set; } = "";


        public Dictionary<ItemType, ushort> Loadout { get; set; } = new Dictionary<ItemType, ushort>();
    }

    public class TeamDefinition
    {
        public string Name { get; set; } = "";
        public string Colour { get; set; } = "#ffffff";
    }

    public class JobConfig
    {
        public bool UseJobSpawnpoint { get; set; } = false;
        public string DefaultJob { get; set; } = "dclass";
        public Dictionary<ItemType, ushort> BaseLoadout { get; set; } = new Dictionary<ItemType, ushort>();


        public Dictionary<string, TeamDefinition> Teams { get; set; } = new Dictionary<string, TeamDefinition>()
        {
            ["government"] = new TeamDefinition()
            {
                Name = "Government",
                Colour = "#5555ff"
            },
            ["rebel"] = new TeamDefinition()
            { 
                Name = "Rebels",
                Colour = "#22aa22"
            },
            ["dclass"] = new TeamDefinition()
            { 
                Name = "D-Class",
                Colour = "#ff4422"
            }


        };

        public Dictionary<string, JobDefinition> Jobs { get; set; } = new Dictionary<string, JobDefinition>()
        {
            ["dclass"] = new JobDefinition()
            {
                Name = "D-Class",
                Description = "Does silly D-Class things",
                Colour = "orange",

                Model = RoleTypeId.ClassD,
                Spawnpoint = RoleTypeId.ClassD,

                Payday = 150,

                Team = "dclass"
            },
            ["gundealer"] = new JobDefinition()
            {
                Name = "Gun Smuggler",
                Description = "Does silly Gun things",
                Colour = "yellow",

                Model = RoleTypeId.Tutorial,
                Spawnpoint = RoleTypeId.ClassD,

                Payday = 200,

                MaxPlayers = 3,

                Team = "dclass"
            },

            ["medic"] = new JobDefinition()
            {
                Name = "Medic",
                Description = "Does silly medic things",
                Colour = "silver",

                Model = RoleTypeId.ClassD,
                Spawnpoint = RoleTypeId.ClassD,

                Loadout = new Dictionary<ItemType, ushort>() { [ItemType.KeycardJanitor] = 1 },
                Payday = 1,

                MaxPlayers = 1,


                Team = "dclass"
            },
            ["scientist"] = new JobDefinition()
            {
                Name = "Scientist",
                Description = "Does silly scientist things",
                Colour = "yellow",

                Model = RoleTypeId.Scientist,
                Spawnpoint = RoleTypeId.Scientist,

                Loadout = new Dictionary<ItemType, ushort>(){ [ItemType.KeycardScientist] = 1 },

                MaxPlayers = 5,

                Payday = 250,

                Team = "government"
            },
            ["security"] = new JobDefinition()
            {
                Name = "Security Guard",
                Description = "Does silly security things",
                Colour = "aqua",

                Model = RoleTypeId.FacilityGuard,
                Spawnpoint = RoleTypeId.Scientist,

                MaxPlayers = 5,

                Loadout = new Dictionary<ItemType, ushort>(){ [ItemType.KeycardGuard] = 1, [ItemType.GunFSP9] = 1, [ItemType.ArmorLight] = 1, [ItemType.Ammo9x19] = 200},

                Payday = 250,

                Team = "government"
            },
            ["overseer"] = new JobDefinition()
            {
                Name = "Overseer",
                Description = "Does silly overseer things",
                Colour = "crimson",

                Model = RoleTypeId.Scientist,
                Spawnpoint = RoleTypeId.Scientist,
                
                Loadout = new Dictionary<ItemType, ushort>(){ [ItemType.KeycardO5] = 1 },

                MaxPlayers = 2,

                Payday = 500,

                Team = "government"
            },
            ["rebel"] = new JobDefinition()
            {
                Name = "Rebel",
                Description = "Does silly rebel things",
                Colour = "light_green",

                Model = RoleTypeId.ChaosConscript,
                Spawnpoint = RoleTypeId.ChaosConscript,

                Loadout = new Dictionary<ItemType, ushort>() { [ItemType.KeycardChaosInsurgency] = 1, [ItemType.GunAK] =1, [ItemType.ArmorHeavy] = 1, [ItemType.GrenadeHE]=1, [ItemType.GunRevolver]=1, [ItemType.Ammo762x39]=200, [ItemType.Ammo44cal]=200 },

                MaxPlayers = 4,

                Payday = 500,

                Team = "rebel"
            }


        };

    }



    public class Job : BaseModule
    {

        public static Job Singleton;

        public Dictionary<Player, string> PlayerRoles = new Dictionary<Player, string>();



        public override void Load()
        {
            Singleton = this;
            PlayerEvents.Joined += Joined;
            PlayerEvents.ChangedRole += Spawned;
            PlayerEvents.Left += Left;
            PlayerEvents.ReceivingLoadout += Loadout;
        }

        public override void Unload()
        {
            PlayerEvents.Joined -= Joined;
            PlayerEvents.ChangedRole -= Spawned;
            PlayerEvents.Left -= Left;
            PlayerEvents.ReceivingLoadout -= Loadout;
        }

        void Joined(PlayerJoinedEventArgs e)
        {
           
        }

        void Left(PlayerLeftEventArgs e)
        {
            if (PlayerRoles.ContainsKey(e.Player))
                PlayerRoles.Remove(e.Player);
        }

        void Spawned(PlayerChangedRoleEventArgs e)
        {
            SendFakeJobBadge(e.Player, e.Player);
            if (e.ChangeReason != RoleChangeReason.LateJoin && e.ChangeReason != RoleChangeReason.Died) return;  

            if (e.ChangeReason == RoleChangeReason.LateJoin)
            {
                SetJob(e.Player, SCPRP.Singleton.Config.JobConfig.DefaultJob);
            }

            if (e.ChangeReason == RoleChangeReason.Died && e.NewRole.RoleTypeId == RoleTypeId.Spectator)
            {
                SetJob(e.Player, e.Player.GetJob());
       
            }

            if (RoleTypeId.ClassD.TryGetRandomSpawnPoint(out Vector3 pos, out float horizontal))
                e.Player.Position = pos;
            
            SendSyncFakeJobBadges(e.Player);

        }

        void Loadout(PlayerReceivingLoadoutEventArgs e)
        {
            e.ClearItems();
            e.ClearAmmo();
            e.InventoryReset = true;

            var job = GetJobInfo(e.Player);
            if (job == null)
                return;

            Dictionary<ItemType, ushort> loadout = new Dictionary<ItemType, ushort>();

            foreach(var pair in SCPRP.Singleton.Config.JobConfig.BaseLoadout)
                loadout.Add(pair.Key, pair.Value);
            foreach (var pair in job.Loadout)
                loadout.Add(pair.Key, pair.Value);

            foreach (var i in loadout)
            {
                if (i.Value == 0) continue;

                if ( Enum.GetName(typeof(ItemType), i.Key).StartsWith("Ammo"))
                {
                    e.AddAmmo(i.Key, i.Value);
                    continue;
                }
                for (int x=0; x < i.Value; x++)
                    e.AddItem(i.Key);
            }
        }
        public static void SetJob(Player player, string role)
        {
            string oldjob = Singleton.PlayerRoles.ContainsKey(player) ? Singleton.PlayerRoles[player] : SCPRP.Singleton.Config.JobConfig.DefaultJob;
            if (!Singleton.PlayerRoles.ContainsKey(player))
                Singleton.PlayerRoles.Add(player, role);

            Singleton.PlayerRoles[player] = role;

            if (SCPRP.Singleton.Config.JobConfig.UseJobSpawnpoint)
                player.SetRole(GetJobInfo(role).Model, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.UseSpawnpoint & RoleSpawnFlags.AssignInventory);
            else
            {
                var oldpos = player.Position;
                player.SetRole(GetJobInfo(role).Model, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.AssignInventory);
                player.Position = oldpos;
            }
            SendFakeJobBadgeAll(player);

            Events.Handlers.PlayerEvents.JobChangedFire(new Events.Arguments.Player.JobChangedEventArgs(player, oldjob, role));
        }

        public static string GetJob(Player player)
        {
            if (!Singleton.PlayerRoles.ContainsKey(player))
                SetJob(player, SCPRP.Singleton.Config.JobConfig.DefaultJob);

            return Singleton.PlayerRoles[player];
        }

        public static JobDefinition GetJobInfo(string job)
        {
            if (!SCPRP.Singleton.Config.JobConfig.Jobs.ContainsKey(job))
                return null;
            return SCPRP.Singleton.Config.JobConfig.Jobs[job];
        }

        public static JobDefinition GetJobInfo(Player player)
        {
            return GetJobInfo(GetJob(player));
        }

        public static List<Player> GetJobPlayers(string job)
        {
            List<Player> result = new List<Player>();
            foreach(var pair in Singleton.PlayerRoles)
            {
                if (pair.Value == job) result.Add(pair.Key);
            }
            return result;
        }
        public static bool IsValidJob(string job)
        {
            return SCPRP.Singleton.Config.JobConfig.Jobs.ContainsKey(job);
        }
        private static void SendFakeJobBadge(Player player, Player targetToTrick)
        {
            if (!targetToTrick.IsReady)
                return;
            if (player == null || player.ReferenceHub == null ||  player.ReferenceHub.serverRoles == null || (!player.IsDummy && player.ReferenceHub.serverRoles.HasGlobalBadge))
                return;

            var jobinfo = GetJobInfo(player);
            if (jobinfo == null) return;

            string text = jobinfo.Name;
            string colour = jobinfo.Colour;

            if (player.ReferenceHub.serverRoles.Network_myText != "" && player.ReferenceHub.serverRoles.Network_myText != null)
                text = player.ReferenceHub.serverRoles.Network_myText + " | " + text;
            try
            {
                targetToTrick.SendFakeSyncVar(player.ReferenceHub.serverRoles.netIdentity, typeof(ServerRoles), "Network_myText", text);
                targetToTrick.SendFakeSyncVar(player.ReferenceHub.serverRoles.netIdentity, typeof(ServerRoles), "Network_myColor", colour);
            }
            catch (Exception e)
            {
                LabApi.Features.Console.Logger.Debug(e.ToString());
            }
        }
        private static void SendFakeJobBadgeAll(Player player)
        {
            foreach(var p in Player.GetAll())
                SendFakeJobBadge(player, p);
        }

        private static void SendSyncFakeJobBadges(Player player)
        {
            foreach (var p in Player.GetAll())
                SendFakeJobBadge(p, player);
        }

        

        public override void Tick()
        {

        }

    }
}
