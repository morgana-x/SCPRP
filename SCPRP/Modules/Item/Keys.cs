using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Wrappers;
using System.Linq;


namespace SCPRP.Modules.Item
{
    public class Keys : BaseModule
    {

        public LabApi.Features.Wrappers.KeycardItem GiveKeys(Player p)
        {
            return KeycardItem.CreateCustomKeycardMetal(p, "Keys", p.DisplayName, "Keys", new KeycardLevels(DoorPermissionFlags.None), UnityEngine.Color.cyan, UnityEngine.Color.blue, UnityEngine.Color.gray, 2, "123456789123");
        }
        public override void Load()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingDoor += InteractingDoor;
            LabApi.Events.Handlers.PlayerEvents.ReceivedLoadout += PlayerSpawned;
            LabApi.Events.Handlers.PlayerEvents.DroppingItem += PlayerDropping;
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker += InteractingLocker;
            LabApi.Events.Handlers.PlayerEvents.InteractingGenerator += InteractingGenerator;
            LabApi.Events.Handlers.ServerEvents.PickupCreated += PickupCreated;
            LabApi.Events.Handlers.PlayerEvents.Dying += OnDeath;
        }
        public override void Unload()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingDoor -= InteractingDoor;
            LabApi.Events.Handlers.PlayerEvents.ReceivedLoadout -= PlayerSpawned;
            LabApi.Events.Handlers.PlayerEvents.DroppingItem -= PlayerDropping;
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker -= InteractingLocker;
            LabApi.Events.Handlers.PlayerEvents.InteractingGenerator -= InteractingGenerator;
            LabApi.Events.Handlers.ServerEvents.PickupCreated -= PickupCreated;
            LabApi.Events.Handlers.PlayerEvents.Dying -= OnDeath;
        }
        public override void Tick()
        {
           
        }

        public  static bool IsKey(LabApi.Features.Wrappers.Item item)
        {
            return item != null && item is LabApi.Features.Wrappers.KeycardItem && ((LabApi.Features.Wrappers.KeycardItem)item).Type == ItemType.KeycardCustomMetalCase;
        }
        void PlayerDropping(PlayerDroppingItemEventArgs e)
        {
            if (e.Player.CurrentItem == null || !IsKey(e.Item))
                return;
            e.IsAllowed = false;
        }
        void PlayerSpawned(PlayerReceivedLoadoutEventArgs e)
        {
            if (e.Player.Role != PlayerRoles.RoleTypeId.Spectator && e.Player.Role != PlayerRoles.RoleTypeId.Filmmaker)
                GiveKeys(e.Player);
        }

        void PickupCreated(PickupCreatedEventArgs e)
        {
            if (e.Pickup.Type == ItemType.KeycardCustomMetalCase)
                e.Pickup.Destroy();
        }
        void InteractingLocker(PlayerInteractingLockerEventArgs e)
        {
            if (IsKey(e.Player.CurrentItem) && !Modules.Entities.Door.Singleton.Config.KeysCanActAsKeycard)
                e.IsAllowed = false;
        }
        void InteractingGenerator(PlayerInteractingGeneratorEventArgs e)
        {
            if (IsKey(e.Player.CurrentItem) && !Modules.Entities.Door.Singleton.Config.KeysCanActAsKeycard)
                e.IsAllowed = false;
        }
        void InteractingDoor(PlayerInteractingDoorEventArgs e)
        {
            var rpdoor = Entities.Door.GetRPDoor(e.Door);
            if (rpdoor == null) return;


            if (e.Player.CurrentItem == null || !IsKey(e.Player.CurrentItem))
                return;


            if (!rpdoor.HasPermission(e.Player))
            {
                if (!Modules.Entities.Door.Singleton.Config.KeysCanActAsKeycard)
                {
                    e.IsAllowed = false;
                }
                if (!rpdoor.Owned)
                    rpdoor.Purchase(e.Player);
                return;
            }
            e.IsAllowed = false;
            rpdoor.Door.Lock(DoorLockReason.AdminCommand, !rpdoor.Door.IsLocked);
        }

        void OnDeath(PlayerDyingEventArgs e)
        {
            if (!e.IsAllowed) return;
            foreach(var i in e.Player.Items.ToList())
            {
                if (IsKey(i))
                    e.Player.RemoveItem(i);
            }
        }
   
    }
}
