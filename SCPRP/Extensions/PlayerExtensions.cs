using LabApi.Features.Wrappers;
using SCPRP.Modules.DB;
using SCPRP.Modules.Players;
using UnityEngine;

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

        public static void Notify(this Player player, string text)
        {
            HUD.ShowHint(player, text);
        }

        public static void SetJob(this Player player, string job)
        {
            Job.SetJob(player, job);
        }
        public static string GetJob(this Player player)
        {
            return Job.GetJob(player);
        }

        public static JobDefinition GetJobInfo(this Player player)
        {
            return Job.GetJobInfo(player);
        }

        public static string GetColouredName(this Player player)
        {
            var job = Job.GetJobInfo(player);
            if (job == null) return $"<color #555555>{player.DisplayName}</color>";
            return $"<color={job.HexColour()}>{player.DisplayName}</color>";
        }

        public static Door GetLookingDoor (this Player pl)
        {
            Vector3 startPos = pl.Camera.position + (pl.Camera.forward * 0.16f);
            for (int i = 0; i < 7; i++)
            {
                foreach (Door v in Door.List)
                {
                    if (v.IsDestroyed) continue;
                    if (v.Zone != pl.Zone) continue;
                    if (!v.Rooms.Contains(pl.Room)) continue;
                    if (Vector3.Distance(v.Position + Vector3.up, startPos) > 0.8f) continue;
                    return v;
                }
                startPos += pl.Camera.forward;
            }
            return null;
        }
    }
}
