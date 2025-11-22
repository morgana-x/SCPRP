using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;


namespace SCPRP.Modules.Item
{
    public class Keys : BaseModule
    {

        public LabApi.Features.Wrappers.KeycardItem GiveKeys(Player p)
        {
            return KeycardItem.CreateCustomKeycardMetal(p, "Keys", p.DisplayName, "Keys", new KeycardLevels(DoorPermissionFlags.All), UnityEngine.Color.cyan, UnityEngine.Color.blue, UnityEngine.Color.gray, 2, "123456789123");
        }
        public override void Load()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingDoor += InteractingDoor;
            LabApi.Events.Handlers.PlayerEvents.ReceivedLoadout += PlayerSpawned;
            LabApi.Events.Handlers.PlayerEvents.DroppingItem += PlayerDropping;
        }
        public override void Unload()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingDoor -= InteractingDoor;
            LabApi.Events.Handlers.PlayerEvents.ReceivedLoadout -= PlayerSpawned;
            LabApi.Events.Handlers.PlayerEvents.DroppingItem -= PlayerDropping;
        }
        public override void Tick()
        {
           
        }

        void PlayerDropping(PlayerDroppingItemEventArgs e)
        {
            var item = e.Item;
            bool holdingkeys = item != null && item is LabApi.Features.Wrappers.KeycardItem && ((LabApi.Features.Wrappers.KeycardItem)item).Type == ItemType.KeycardCustomMetalCase;

            if (e.Player.CurrentItem == null || !holdingkeys)
                return;
            e.IsAllowed = false;
        }
        void PlayerSpawned(PlayerReceivedLoadoutEventArgs e)
        {
            if (e.Player.Role != PlayerRoles.RoleTypeId.Spectator && e.Player.Role != PlayerRoles.RoleTypeId.Filmmaker)
                GiveKeys(e.Player);
        }
        void InteractingDoor(PlayerInteractingDoorEventArgs e)
        {
            var rpdoor = Entities.Door.GetRPDoor(e.Door);
            if (rpdoor == null) return;

            var item = e.Player.CurrentItem;
            bool holdingkeys = item != null && item is LabApi.Features.Wrappers.KeycardItem && ((LabApi.Features.Wrappers.KeycardItem)item).Type == ItemType.KeycardCustomMetalCase;

            if (e.Player.CurrentItem == null || !holdingkeys)
                return;


            if (!rpdoor.HasPermission(e.Player))
            {
                if (!SCPRP.Singleton.Config.DoorsConfig.KeysCanActAsKeycard)
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
   
    }
}
