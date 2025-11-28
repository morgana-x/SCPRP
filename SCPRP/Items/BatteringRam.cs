using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;
using SCPRP.Modules.Items;
using SCPRP.Modules.Players.Jobs;
using System;
using Utils;

namespace SCPRP.Items
{
    public class BatteringRam : CustomItemBase
    {
        public BatteringRam() : base()
        {
        }

        public override ItemType BaseItem => ItemType.ParticleDisruptor;

        DateTime nextFire = DateTime.Now;

        public override void OnAim(Player player)
        {
            
        }


        public override void OnReload(Player player)
        {
            
        }

        public override void OnGive(Player player)
        {
           ((ParticleDisruptorItem)Item).StoredAmmo = ((ParticleDisruptorItem)Item).MaxAmmo;
      

        }

        public override void OnUnequip(Player player)
        {
           
        }
        public override void OnEquip(Player player)
        {
            
        }
        public override void OnUsing(Player player)
        {

        }

        public override void OnTick(Player player)
        {

        }

        public override void OnShoot(Player player)
        {
            if (DateTime.Now < nextFire) return;

            ((ParticleDisruptorItem)Item).StoredAmmo = ((ParticleDisruptorItem)Item).MaxAmmo;
            nextFire = DateTime.Now.AddSeconds(5);
            var lookingDoor = player.GetLookingDoor();
            if (lookingDoor == null)
                return;

            if (lookingDoor.IsOpened)
                return;

            RPDoor door = Modules.Entities.Door.GetRPDoor(lookingDoor);
            if (door == null) return;

            if (door.Teams.Contains("world"))
                return;

            if (door.Owner != null && !Government.IsWarranted(door.Owner))
            {
                player.NotifyTop("<color=red>Player isn't warranted! .warrant them!!!</color>");
                return;
            }

            ExplosionUtils.ServerSpawnEffect(lookingDoor.Position + new UnityEngine.Vector3(0, 1, 0), ItemType.GrenadeHE);
            lookingDoor.IsOpened = true;
          //  lookingDoor.Lock(Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand, true);
        }
    }
}
