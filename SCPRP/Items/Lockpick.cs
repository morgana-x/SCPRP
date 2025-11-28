using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;
using SCPRP.Modules.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPRP.Items
{
    public class Lockpick : CustomItemBase
    {
        public override ItemType BaseItem => ItemType.KeycardChaosInsurgency;

        RPDoor LockpickingDoor = null;
        DateTime LockpickingEnd = DateTime.Now;
        DateTime nextHint = DateTime.Now;
        DateTime nextCheckDoor = DateTime.Now;
        public virtual int LockpickingDuration => 12;

        public override void OnAim(Player player)
        {

        }
        public override void OnUnequip(Player player)
        {
            LockpickingDoor = null;
        }
        public override void OnEquip(Player player)
        {
            var door = player.GetLookingDoor();
            if (door == null) return;
            if (door.IsOpened) return;

            var rpdoor = Modules.Entities.Door.GetRPDoor(door);
            if (rpdoor == null) return;

            if (LockpickingDoor != null && rpdoor == LockpickingDoor) return;

            LockpickingEnd = DateTime.Now.AddSeconds(LockpickingDuration);
            LockpickingDoor = rpdoor;
            player.Notify("Lockpicking...");
        }
        public override void OnGive(Player player)
        {

        }
        public override void OnUsing(Player player)
        {
           
        }

        public override void OnReload(Player player)
        {

        }

        public override void OnShoot(Player player)
        {

        }

        public override void OnTick(Player player)
        {
            if (LockpickingDoor == null)
                return;
           
            if (DateTime.Now >= LockpickingEnd)
            {
                LockpickingDoor.Door.IsOpened = true;
                LockpickingDoor = null;
                player.Notify("Lockpicked door!");
                return;
            }

            if (DateTime.Now > nextCheckDoor)
            {
                nextCheckDoor = DateTime.Now.AddSeconds(1);
                if (player.GetLookingDoor() != LockpickingDoor.Door)
                {
                    LockpickingDoor = null;
                    player.Notify("Cancelled lockpicking!");
                    return;
                }
            }

            if (DateTime.Now >= nextHint)
            {
                nextHint = DateTime.Now.AddSeconds(4);
                player.Notify($"Lockpicking {(int)Math.Round((1f-(LockpickingEnd.Subtract(DateTime.Now).TotalSeconds / LockpickingDuration)) * 100)}%...");
            }

     
        }
    }
}
