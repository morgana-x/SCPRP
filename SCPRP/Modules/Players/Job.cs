using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace SCPRP.Modules.Players
{
    public class JobDefinition
    {
        
        public string Name = "Rolename";
        public string Description = "A role";
        public Color Colour = Color.gray;

        public RoleTypeId Model = RoleTypeId.ClassD;

        public RoleTypeId Spawnpoint = RoleTypeId.None;

        public int Payday { get; set; } = 0;
        public int MaxPlayers { get; set; } = 0;
        public string Team { get; set; } = "";


        public Dictionary<ItemType, int> Loadout { get; set; } = new Dictionary<ItemType, int>();
    }

    public class TeamDefinition
    {
        public string Name { get; set; } = "";
        public Color Colour { get; set; } = Color.gray;
    }

    public class JobConfig
    {
        public bool RepawnOnRoleChange { get; set; } = false;
        public string DefaultRole { get; set; } = "dclass";
        public Dictionary<ItemType, int> BaseLoadout { get; set; } = new Dictionary<ItemType, int>();


        public Dictionary<string, TeamDefinition> Teams { get; set; } = new Dictionary<string, TeamDefinition>()
        {
            ["government"] = new TeamDefinition()
            {
                Name = "Government",
                Colour = Color.blue
            },
            ["rebel"] = new TeamDefinition()
            { 
                Name = "Rebels",
                Colour = Color.green
            },
            ["dclass"] = new TeamDefinition()
            { 
                Name = "D-Class",
                Colour = new Color(255,100,10)
            }


        };

        public Dictionary<string, JobDefinition> Jobs { get; set; } = new Dictionary<string, JobDefinition>()
        {
            ["dclass"] = new JobDefinition()
            {
                Name = "D-Class",
                Description = "Does silly D-Class things",
                Colour = new Color(251, 81, 43),

                Model = RoleTypeId.ClassD,
                Spawnpoint = RoleTypeId.ClassD,

                Payday = 1,

                Team = "dclass"
            },
            ["janitor"] = new JobDefinition()
            {
                Name = "Janitor",
                Description = "Does silly Janitor things",
                Colour = new Color(251, 81, 43),

                Model = RoleTypeId.ClassD,
                Spawnpoint = RoleTypeId.ClassD,

                Payday = 1,

                Team = "dclass"
            },
            ["scientist"] = new JobDefinition()
            {
                Name = "Scientist",
                Description = "Does silly scientist things",
                Colour = new Color(245, 220, 84),

                Model = RoleTypeId.Scientist,
                Spawnpoint = RoleTypeId.Scientist,

                Loadout = new Dictionary<ItemType, int>(){ [ItemType.KeycardScientist] = 1 },

                MaxPlayers = 5,

                Payday = 250,

                Team = "Facility"
            },
            ["security"] = new JobDefinition()
            {
                Name = "Security Guard",
                Description = "Does silly scientist things",
                Colour = new Color(245, 220, 84),

                Model = RoleTypeId.Scientist,
                Spawnpoint = RoleTypeId.Scientist,

                MaxPlayers = 5,

                Loadout = new Dictionary<ItemType, int>(){ [ItemType.KeycardGuard] = 1, [ItemType.GunFSP9] = 1, [ItemType.ArmorLight] = 1, [ItemType.Ammo9x19] = 200},

                Payday = 250,

                Team = "Facility"
            },
            ["overseer"] = new JobDefinition()
            {
                Name = "Overseer",
                Description = "Does silly overseer things",
                Colour = new Color(255, 5, 181),

                Model = RoleTypeId.Scientist,
                Spawnpoint = RoleTypeId.Scientist,
                
                Loadout = new Dictionary<ItemType, int>(){ [ItemType.KeycardO5] = 1 },

                MaxPlayers = 2,

                Payday = 500,

                Team = "Facility"
            },

        };

    }



    public class Job : BaseModule
    {

        public static Job Singleton;

        public Dictionary<Player, string> PlayerRoles = new Dictionary<Player, string>();
        public Dictionary<Player, string> PlayerBadges = new Dictionary<Player, string>();
        public override void Load()
        {
            Singleton = this;
            PlayerEvents.Joined += Joined;
            PlayerEvents.Left += Left;
        }

        public override void Unload()
        {
            PlayerEvents.Joined -= Joined;
            PlayerEvents.Left -= Left;
        }

        void Joined(PlayerJoinedEventArgs e)
        {
           // PlayerBadges.Add(e.Player, e.Player.ReferenceHub.)
        }

        void Left(PlayerLeftEventArgs e)
        {
            if (PlayerRoles.ContainsKey(e.Player))
                PlayerRoles.Remove(e.Player);
            if (PlayerBadges.ContainsKey(e.Player))
                PlayerBadges.Remove(e.Player);
        }

        public static void SetRole(Player player, string role)
        {
            if (!Singleton.PlayerRoles.ContainsKey(player))
                Singleton.PlayerRoles.Add(player, role);

            Singleton.PlayerRoles[player] = role;
        }

        public static string GetJob(Player player)
        {
            if (!Singleton.PlayerRoles.ContainsKey(player))
                SetRole(player, SCPRP.Singleton.Config.JobConfig.DefaultRole);

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

        public override void Tick()
        {

        }

    }
}
