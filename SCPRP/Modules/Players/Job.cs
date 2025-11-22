using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;
using Exiled.API.Extensions;
using LabApi.Features.Extensions;
using SCPRP.Extensions;
using System.Linq;
using System.Globalization;
using Unity.Jobs;

namespace SCPRP.Modules.Players
{
    public class JobDefinition
    {
        
        public string Name { get; set; }  = "Rolename";
        public string Description { get; set; }  = "A role";
        public string Colour { get; set; } = "#ffffff";

        public RoleTypeId Model { get; set; } = RoleTypeId.ClassD;

        public SpawnDefinition Spawnpoint { get; set; }  = new SpawnDefinition();

        public int Payday { get; set; } = 0;
        public int MaxPlayers { get; set; } = 0;
        public string Team { get; set; } = "";

        public bool ForceUseSpawnpoint { get; set; } = false;


        public Dictionary<ItemType, ushort> Loadout { get; set; } = new Dictionary<ItemType, ushort>();
        static Vector3 HexToCol(string colour)
        {
            return new Vector3(byte.Parse(colour.Substring(1, 2), NumberStyles.AllowHexSpecifier), byte.Parse(colour.Substring(3, 2), NumberStyles.AllowHexSpecifier), byte.Parse(colour.Substring(5, 2), NumberStyles.AllowHexSpecifier));
        }
        static string NearestBadgeInfoColour(string hexcolour)
        {
            string nearestColour = Misc.AcceptedColours.First();
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < Misc.AcceptedColours.Length; i++)
            {
                float dist = (HexToCol("#" + Misc.AcceptedColours[i]) - HexToCol(hexcolour)).magnitude;
                if (dist >= nearestDistance) continue;
                nearestDistance = dist;
                nearestColour = Misc.AcceptedColours[i];
            }
            return "#" + nearestColour;
        }
        public string HexColour()
        {
            string colour = Colour;
            if (UnityEngine.ColorUtility.TryParseHtmlString(Colour, out Color col))
                colour = col.ToHex(); //NearestBadgeInfoColour(col.ToHex());

            if (!colour.StartsWith("#"))
            {
                switch (Colour)
                {

                    case "silver":
                        colour = "#c4c4c4";
                        break;
                    case "aqua":
                        colour = "#16e6f3";
                        break;
                    case "crimson":
                        colour = "#f3163e";
                        break;
                    case "light_green":
                        colour = "#71f316";
                        break;
                    case "magenta":
                        colour = "#ff00ff";
                        break;
                }
            }
            if (colour.StartsWith("#"))
                return NearestBadgeInfoColour(colour);
            return Colour;
        }
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

        public int PaydayIntervalSeconds { get; set; } = 600;
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
                Colour = "#ff891d"
            },
            ["world"] = new TeamDefinition()
            {
                Name = "",
                Colour = "#ffffff"
            },
            ["criminals"] = new TeamDefinition()
            {
                Name = "Criminals",
                Colour = "#ff2222"
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

                Payday = 150,

                Team = "dclass"
            },
            ["gundealer"] = new JobDefinition()
            {
                Name = "Gun Smuggler",
                Description = "Sells smuggled weapons",
                Colour = "yellow",

                Model = RoleTypeId.Tutorial,

                Payday = 200,

                MaxPlayers = 3,

                Team = "dclass"
            },

            ["keycard"] = new JobDefinition()
            {
                Name = "Keycard Forger",
                Description = "Forges fake cards",
                Colour = "yellow",

                Model = RoleTypeId.Tutorial,

                Payday = 1,

                MaxPlayers = 1,

                Team = "criminals"
            },

            ["medic"] = new JobDefinition()
            {
                Name = "Medic",
                Description = "Sells medical supplies",
                Colour = "aqua",

                Model = RoleTypeId.ClassD,

                Loadout = new Dictionary<ItemType, ushort>() { [ItemType.KeycardJanitor] = 1 },
                Payday = 1,

                MaxPlayers = 1,


                Team = "dclass"
            },
            ["thief"] = new JobDefinition()
            {
                Name = "Thief",
                Description = "Pillage and Plunder!! (The printers and weapons!)",
                Colour = "magenta",

                Model = RoleTypeId.ClassD,

                Payday = 150,

                MaxPlayers = 5,

                Team = "criminals"
            },
            ["hitman"] = new JobDefinition()
            {
                Name = "Hitman",
                Description = "Get that sweet blood money",
                Colour = "red",

                Model = RoleTypeId.Tutorial,

                Loadout = new Dictionary<ItemType, ushort>() { [ItemType.KeycardScientist] = 1, [ItemType.GunCOM15] = 1, [ItemType.Ammo9x19]=20},
                Payday = 150,

                MaxPlayers = 2,

                Team = "criminals"
            },
            ["drug"] = new JobDefinition()
            {
                Name = "Drug Dealer",
                Description = "Mmmm drugs",
                Colour = "red",

                Model = RoleTypeId.Tutorial,

                Loadout = new Dictionary<ItemType, ushort>() { },
                Payday = 150,

                MaxPlayers = 3,

                Team = "criminals"
            },
            ["scientist"] = new JobDefinition()
            {
                Name = "Scientist",
                Description = "A very bad scientist, both morally and literally.",
                Colour = "yellow",

                Model = RoleTypeId.Scientist,
                Spawnpoint = new SpawnDefinition(RoleTypeId.Scientist),

                Loadout = new Dictionary<ItemType, ushort>(){ [ItemType.KeycardScientist] = 1 },

                MaxPlayers = 5,

                Payday = 250,

                Team = "government"
            },
            ["security"] = new JobDefinition()
            {
                Name = "Security Guard",
                Description = "Keeps law and order in the facility",
                Colour = "silver",

                Model = RoleTypeId.FacilityGuard,

                MaxPlayers = 5,

                Loadout = new Dictionary<ItemType, ushort>(){ [ItemType.KeycardGuard] = 1, [ItemType.GunFSP9] = 1, [ItemType.ArmorLight] = 1, [ItemType.Ammo9x19] = 70},

                Payday = 250,

                Team = "government"
            },
            ["bodyguard"] = new JobDefinition()
            {
                Name = "Mayor's Bodyguard",
                Description = "Protects the mayor",
                Colour = "silver",

                Model = RoleTypeId.NtfSergeant,

                MaxPlayers = 1,

                Loadout = new Dictionary<ItemType, ushort>() { [ItemType.KeycardGuard] = 1, [ItemType.GunE11SR] = 1, [ItemType.ArmorHeavy] = 1, [ItemType.Ammo556x45] = 70 },

                Payday = 250,

                Team = "government"
            },
            ["overseer"] = new JobDefinition()
            {
                Name = "Overseer",
                Description = "Controls the facility (>:3), Guards must follow their orders",
                Colour = "crimson",

                Model = RoleTypeId.Scientist,
                
                Loadout = new Dictionary<ItemType, ushort>(){ [ItemType.KeycardO5] = 1 },

                MaxPlayers = 1,

                Payday = 500,

                Team = "government"
            },
            ["rebel"] = new JobDefinition()
            {
                Name = "Rebel",
                Description = "Hates the evil facility and loves D-Class!",
                Colour = "light_green",

                Model = RoleTypeId.ChaosConscript,
                Spawnpoint = new SpawnDefinition(RoleTypeId.ChaosConscript),

                Loadout = new Dictionary<ItemType, ushort>() { [ItemType.KeycardChaosInsurgency] = 1, [ItemType.GunAK] =1, [ItemType.ArmorHeavy] = 1, [ItemType.GunRevolver]=1, [ItemType.Ammo762x39]=50, [ItemType.Ammo44cal]=10 },

                MaxPlayers = 4,

                Payday = 500,

                Team = "rebel",

                ForceUseSpawnpoint = true
            }


        };

    }



    public class Job : BaseModule
    {

        public static Job Singleton;

        public static JobConfig Config { get { return SCPRP.Singleton.Config.JobConfig; } }

        public Dictionary<Player, string> PlayerRoles = new Dictionary<Player, string>();



        public override void Load()
        {
            Singleton = this;
            PlayerEvents.Joined += Joined;
            PlayerEvents.ChangedRole += Spawned;
            PlayerEvents.Left += Left;
            PlayerEvents.ReceivingLoadout += Loadout;
            PlayerEvents.ThrowingItem += Throwing;
            PlayerEvents.DroppingItem += Dropping;
            PlayerEvents.DroppingAmmo += DroppingAmmo;
            PlayerEvents.ChangedBadgeVisibility += BadgeChanged;
            PlayerEvents.GroupChanged += GroupChanged;
            PlayerEvents.Dying += OnDeath;
        }

        public override void Unload()
        {
            PlayerEvents.Joined -= Joined;
            PlayerEvents.ChangedRole -= Spawned;
            PlayerEvents.Left -= Left;
            PlayerEvents.ReceivingLoadout -= Loadout;
            PlayerEvents.ThrowingItem -= Throwing;
            PlayerEvents.DroppingItem -= Dropping;
            PlayerEvents.DroppingAmmo -= DroppingAmmo;
            PlayerEvents.ChangedBadgeVisibility -= BadgeChanged;
            PlayerEvents.GroupChanged -= GroupChanged;
            PlayerEvents.Dying -= OnDeath;
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
      
            if (e.ChangeReason != RoleChangeReason.LateJoin && e.ChangeReason != RoleChangeReason.Died) return;  

            if (e.ChangeReason == RoleChangeReason.LateJoin)
            {
                SetJob(e.Player, SCPRP.Singleton.Config.JobConfig.DefaultJob);
            }

            if (e.ChangeReason == RoleChangeReason.Died && e.NewRole.RoleTypeId == RoleTypeId.Spectator)
            {
                SetJob(e.Player, e.Player.GetJob());
       
            }
            var spawn = SCPRP.Singleton.Config.MapConfig.Spawnpoint;
            if (spawn.GetSpawnPosition() == UnityEngine.Vector3.zero)
                spawn = new SpawnDefinition(RoleTypeId.ClassD);

           
            e.Player.Position = spawn.GetSpawnPosition();
            
            SendSyncFakeJobBadges(e.Player);
            SendFakeJobBadgeAll(e.Player);
            e.Player.InfoArea &= ~PlayerInfoArea.PowerStatus;
            e.Player.InfoArea &= ~PlayerInfoArea.UnitName;
            e.Player.InfoArea &= ~PlayerInfoArea.Role;
            e.Player.InfoArea &= ~PlayerInfoArea.Badge;
            e.Player.CustomInfo = GetColouredJobName(e.Player.GetJob());
        }

        void BadgeChanged(PlayerChangedBadgeVisibilityEventArgs e)
        {
            SendFakeJobBadgeAll(e.Player);
        }

        void GroupChanged(PlayerGroupChangedEventArgs e)
        {
            SendFakeJobBadgeAll(e.Player);
        }

        bool shouldDrop(Player e, ItemType type)
        {
            var jobinfo = e.GetJobInfo();
            if (jobinfo == null) return true;
            var loadout = jobinfo.Loadout;
            if (!loadout.ContainsKey(type)) return true;
            var numInventoryItem = e.Inventory.UserInventory.Items.Where((x => { return x.Value.ItemTypeId == type; })).Count();
            var numLoadoutItem = loadout.Where((x => { return x.Key == type; })).Count();
            if (numInventoryItem > numLoadoutItem)
                return true;
            return false;
        }
        void Dropping(PlayerDroppingItemEventArgs e)
        {
            if (!shouldDrop(e.Player, e.Item.Type))
            {
                e.IsAllowed = false;
                HUD.ShowHint(e.Player, "<color=red>Cannot drop loadout item!</color>");
            }
        }

        void DroppingAmmo(PlayerDroppingAmmoEventArgs e)
        {
            var jobinfo = e.Player.GetJobInfo();
            if (jobinfo == null) return;
            var loadout = jobinfo.Loadout;
            if (!loadout.ContainsKey(e.Type)) return;
            e.IsAllowed = false;
        }


        void Throwing(PlayerThrowingItemEventArgs e)
        {
            if (!shouldDrop(e.Player, e.Pickup.Type))
            {
                e.IsAllowed = false;
                HUD.ShowHint(e.Player, "<color=red>Cannot drop loadout item!</color>");
            }
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

            if (SCPRP.Singleton.Config.JobConfig.UseJobSpawnpoint || GetJobInfo(role).ForceUseSpawnpoint)
            {
                player.SetRole(GetJobInfo(role).Model, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.UseSpawnpoint & RoleSpawnFlags.AssignInventory);
                
                var spawnp = GetJobInfo(role).Spawnpoint;
                if (spawnp.GetSpawnPosition() == Vector3.zero)
                    spawnp = SCPRP.Singleton.Config.MapConfig.Spawnpoint;

                player.Position = spawnp.GetSpawnPosition();
            }
            else
            {
                var oldpos = player.Position;
                player.SetRole(GetJobInfo(role).Model, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.AssignInventory);
                player.Position = oldpos;
            }
            SendFakeJobBadgeAll(player);
            player.CustomInfo = GetColouredJobName(player.GetJob());
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

        public static string GetColouredJobName(string job)
        {
            var def = GetJobInfo(job);
            if (def == null) return job;
            return $"<color={def.HexColour()}>{def.Name}</color>";
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

        public static bool IsValidTeam(string team)
        {
            return SCPRP.Singleton.Config.JobConfig.Teams.ContainsKey(team);
        }
        public static string GetColouredTeamName(string team)
        {
            if (!IsValidTeam(team)) return team;
            return $"<color={Config.Teams[team].Colour}>{Config.Teams[team].Name}</color>";
        }

        private static void SendFakeJobBadge(Player player, Player targetToTrick)
        {
            if (!targetToTrick.IsReady)
                return;
            if (player == null || player.ReferenceHub == null ||  player.ReferenceHub.serverRoles == null || (!player.IsDummy && player.ReferenceHub.serverRoles.HasGlobalBadge))
                return;
            if (player.ReferenceHub.serverRoles.Network_myText != "")
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
            foreach (var p in Player.GetAll())
                SendFakeJobBadge(player, p);
        }

        private static void SendSyncFakeJobBadges(Player player)
        {
            foreach (var p in Player.GetAll())
                SendFakeJobBadge(p, player);
        }
        void OnDeath(PlayerDyingEventArgs e)
        {
            if (!e.IsAllowed) return;
            var job = GetJobInfo(e.Player);
            if (job == null) return;
            foreach (var i in e.Player.Items.ToList())
            {
                if (job.Loadout.ContainsKey(i.Type))
                    e.Player.RemoveItem(i);
            }
        }

        DateTime nextPayday = DateTime.Now;
        public override void Tick()
        {
            if (DateTime.Now > nextPayday)
            {
                nextPayday = DateTime.Now.AddSeconds(SCPRP.Singleton.Config.JobConfig.PaydayIntervalSeconds);
                foreach(var p in Player.GetAll())
                {
                    var job = p.GetJobInfo();
                    if (job == null) continue;
                    if (job.Payday == 0) continue;
                    p.AddMoney(job.Payday);
                    HUD.ShowHint(p, $"Payday! Received ${job.Payday}!");
                }
            }
        }

    }
}
