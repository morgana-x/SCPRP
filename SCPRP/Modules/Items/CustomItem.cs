using CommandSystem.Commands.RemoteAdmin.Inventory;
using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SCPRP.Modules.Items
{
    public abstract class CustomItemBase
    {
        public abstract ItemType BaseItem { get; }
        public virtual bool CanDrop => false;
        public Item Item;
        public abstract void OnShoot(Player player);
        public abstract void OnAim(Player player);
        public abstract void OnReload(Player player);
        public abstract void OnGive(Player player);
        public abstract void OnEquip(Player player);
        public abstract void OnUnequip(Player player);

        public abstract void OnUsing(Player player);

        public abstract void OnTick(Player player);
        public CustomItemBase()
        {
        }
    }

    public class CustomItem : BaseModule
    {
        public Dictionary<string, Type> CustomItemTypes = new Dictionary<string, Type>();
        public Dictionary<Item, CustomItemBase> SpawnedCustomItems = new Dictionary<Item, CustomItemBase>();

        public static CustomItem Singleton;
        public override void Load()
        {
            Singleton = this;

            ServerEvents.PickupDestroyed += PickupDestroyed;
            PlayerEvents.DroppingItem += DroppingItem;
            PlayerEvents.ShootingWeapon += ShootingWeapon;
            PlayerEvents.AimedWeapon += AimedWeapon;
            PlayerEvents.ReloadingWeapon += ReloadingWeapon;
            PlayerEvents.ThrowingItem += ThrowingItem;
            PlayerEvents.ChangedItem += ChangedItem;
            PlayerEvents.UsingItem += UsingItem;

            var classes = Assembly.GetExecutingAssembly()
                   .GetTypes()
                   .Where(t => t.IsClass && t.Namespace.StartsWith("SCPRP.Items"))
                   .ToList();

            foreach (var type in classes.Where((x) => { return x.IsSubclassOf(typeof(CustomItemBase)); }))
            {
                CustomItemTypes.Add(type.Name, type);
                Logger.Info("Registed item " + type.Name);
            }
        }

        public override void Unload()
        {
            ServerEvents.PickupDestroyed -= PickupDestroyed;
            PlayerEvents.DroppingItem -= DroppingItem;
            PlayerEvents.ShootingWeapon -= ShootingWeapon;
            PlayerEvents.AimedWeapon -= AimedWeapon;
            PlayerEvents.ReloadingWeapon -= ReloadingWeapon;
            PlayerEvents.ThrowingItem -= ThrowingItem;
            PlayerEvents.ChangedItem -= ChangedItem;
            PlayerEvents.UsingItem -= UsingItem;
        }

        public override void Tick()
        {
            foreach (var item in SpawnedCustomItems)
            {
                if (item.Key == null) continue;
                if (item.Value == null) continue;
                if (!item.Key.IsEquipped) continue;
                item.Value.OnTick(item.Key.CurrentOwner);
            }
        }

        public static CustomItemBase GiveItem(Player player, Type type, ItemAddReason reason = ItemAddReason.AdminCommand)
        {
            CustomItemBase item = (CustomItemBase)Activator.CreateInstance(type);
            Item baseitem = player.AddItem(item.BaseItem, reason);
            item.Item = baseitem;
            Singleton.SpawnedCustomItems.Add(baseitem, item);
            item.OnGive(player);
            return item;
        }

        public static CustomItemBase GiveItem(Player player, string type, ItemAddReason reason = ItemAddReason.AdminCommand)
        {
            if (!Singleton.CustomItemTypes.ContainsKey(type))
                return null;
            return GiveItem(player, Singleton.CustomItemTypes[type], reason);
        }
        void ChangedItem(PlayerChangedItemEventArgs e)
        {
            var newcustomitem = GetCustomItem(e.NewItem);
            if (newcustomitem != null)
            {
                e.Player.Notify($"Selected <color=yellow>{newcustomitem.GetType().Name}</color>");
                newcustomitem.OnEquip(e.Player);
            }
            var oldcustomitem = GetCustomItem(e.OldItem);
            if (oldcustomitem != null)
            {
                oldcustomitem.OnUnequip(e.Player);
            }
        }
        void UsingItem(PlayerUsingItemEventArgs e)
        {
            var newcustomitem = GetCustomItem(e.Item);
            if (newcustomitem != null)
            {
                e.IsAllowed = false;
                newcustomitem.OnUsing(e.Player);
            }
        }
        void DroppingItem(PlayerDroppingItemEventArgs e)
        {
            if (Singleton.SpawnedCustomItems.ContainsKey(e.Item))
                e.IsAllowed = false;
        }
        public static Item GetItemFromID(ItemIdentifier id)
        {
            foreach (var item in Singleton.SpawnedCustomItems.ToList())
                if (item.Key.Serial == id.SerialNumber)
                    return item.Key;
            return null;
        }
        public static CustomItemBase GetCustomItem(Item itm)
        {
            foreach (var item in Singleton.SpawnedCustomItems.ToList())
                if (item.Key == itm)
                    return item.Value;
            return null;
        }

        void ShootingWeapon(PlayerShootingWeaponEventArgs e)
        {
            var item = GetItemFromID(e.FirearmItem.Base.ItemId);
            if (item == null) return;
            if (Singleton.SpawnedCustomItems.ContainsKey(item))
            {
                e.IsAllowed = false;
                Singleton.SpawnedCustomItems[e.FirearmItem].OnShoot(e.Player);
            }
        }
        void AimedWeapon(PlayerAimedWeaponEventArgs e)
        {
            var item = GetItemFromID(e.FirearmItem.Base.ItemId);
            if (item == null) return;
            if (Singleton.SpawnedCustomItems.ContainsKey(item))
            {
                Singleton.SpawnedCustomItems[item].OnAim(e.Player);
            }
        }
        void ReloadingWeapon(PlayerReloadingWeaponEventArgs e)
        {
            var item = GetItemFromID(e.FirearmItem.Base.ItemId);
            if (item == null) return;
            if (Singleton.SpawnedCustomItems.ContainsKey(item))
            {
                e.IsAllowed = false;
                Singleton.SpawnedCustomItems[item].OnReload(e.Player);
            }
        }

        void ThrowingItem(PlayerThrowingItemEventArgs e)
        {
            var i = GetItemFromID(e.Pickup.Base.ItemId);
            if (i == null) return;
            if (Singleton.SpawnedCustomItems.ContainsKey(i))
                e.IsAllowed = false;
        }
        void PickupDestroyed(PickupDestroyedEventArgs e)
        {
            var i = GetItemFromID(e.Pickup.Base.ItemId);
            if (i == null) return;
            if (SpawnedCustomItems.ContainsKey(i))
                SpawnedCustomItems.Remove(i);
        }
    }
}
