using System.Collections.Generic;
using System.Linq;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using UnityEngine;
namespace SCPRP.Modules.Players
{
    public class Pocket : BaseModule
    {
        public static Pocket Singleton;

        public Dictionary<Player, List<BaseEntity>> PocketInventory = new Dictionary<Player, List<BaseEntity>>();

        public class PocketConfig 
        {
            public int MaxCapacity { get; set; } = 3;
        }
        public override void Load()
        {
            Singleton = this;
            PlayerEvents.Left += Left;
            PlayerEvents.Dying += Death;
        }

        public override void Unload()
        {
            PlayerEvents.Left -= Left;
            PlayerEvents.Dying -= Death;
        }

        public static List<BaseEntity> GetInventory(Player player)
        {
            if (!Singleton.PocketInventory.ContainsKey(player))
                return new List<BaseEntity>();

            return Singleton.PocketInventory[player];
        }
        public static void Pickup(Player player, BaseEntity entity)
        {
            if (GetInventory(player).Contains(entity))
                return;
            if (GetInventory(player).Count >= SCPRP.Singleton.Config.PocketConfig.MaxCapacity)
                return;

            if (!Singleton.PocketInventory.ContainsKey(player))
                Singleton.PocketInventory.Add(player, new List<BaseEntity>());
            Singleton.PocketInventory[player].Add(entity);

            entity.CoreObject.transform.position = new UnityEngine.Vector3(-16.820f, 315.462f, -32.034f);
            entity.Paused = true;
        }
        public static void Drop(Player player, int yoffset=0)
        {
            if (!Singleton.PocketInventory.ContainsKey(player)) return;
            var inv = Singleton.PocketInventory[player];
            if (inv.Count <= 0) return;
            var ent = inv.Last();
            inv.Remove(ent);
            ent.CoreObject.transform.position = (player.IsAlive ? (player.Camera.position + (player.Camera.forward * 0.8f)) : (player.Position + (Vector3.up * 0.5f))) + ((Vector3.up * 0.85f) * yoffset);
            ent.Paused = false;
        }

        public static void DropAll(Player player)
        {
            if (!Singleton.PocketInventory.ContainsKey(player)) return;

            for (int i=0; i<GetInventory(player).Count; i++)
                Drop(player, i);
        }
        void Left(PlayerLeftEventArgs e)
        {
            DropAll(e.Player);
            if (PocketInventory.ContainsKey(e.Player))
                PocketInventory.Remove(e.Player);
        }
        void Death(PlayerDyingEventArgs e)
        {
            DropAll(e.Player);
        }
        public override void Tick()
        {
        }
    }
}
