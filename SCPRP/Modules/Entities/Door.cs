using CommandSystem.Commands.RemoteAdmin.Doors;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using UserSettings.ServerSpecific;

namespace SCPRP.Modules.Entities
{

    public class DoorsConfig
    {
        public bool KeysCanActAsKeycard { get; set; } = false;
        public int MaxDoors { get; set; } = 4;
    }

    public class RPDoor
    {
        public LabApi.Features.Wrappers.Door Door;
        public int Price;
        public string Name = "";

        public TextToy[] TextScreens;

        public List<string> Teams = new List<string>();
        public Player Owner = null;
        public List<Player> Coowners = new List<Player>();


        public DateTime nextRepair = DateTime.Now;
        public RPDoor(LabApi.Features.Wrappers.Door door, int price=1000)
        {
            Door = door;
            Price = price;
            TextScreens = new TextToy[2];
            TextScreens[0] = TextToy.Create((new UnityEngine.Vector3(0,0,1) * 0.25f) + (new UnityEngine.Vector3(0, 1, 0) * 1.25f), UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 180, 0)), door.Transform);
            TextScreens[1] = TextToy.Create((new UnityEngine.Vector3(0,0,-1) * 0.25f) + (new UnityEngine.Vector3(0, 1, 0) * 1.25f), UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 0, 0)), door.Transform);
            UpdateText();
            TextVisible(true);
        }

        public bool Owned => Teams.Count > 0 || Owner != null;
        private void UpdateText()
        {
            string text = $"{Name}<br>";

           
            if (!Owned)
                text += $"Press F2 to purchase<br><color #55ff55>${Price}</color><br>";

            foreach (var t in Teams)
                text += $"{t}</align><br>";

            if (Owner != null) 
                text += $"<color {Owner.RoleBase.RoleColor.ToHex()}>{Owner.DisplayName}</color><br>";
            foreach (var coowner in Coowners)
                text += $"<color {coowner.RoleBase.RoleColor.ToHex()}>{coowner.DisplayName}</color><br>";

            foreach (var t in TextScreens)
            {
                t.TextFormat = text;
                t.Spawn();
            }

        }

        public void SetOwner(Player p)
        {
            Owner = p;
            Coowners.Clear();
            if (Owner == null && Door.IsLocked) { Door.Lock(DoorLockReason.AdminCommand, false); }
            UpdateText();
        }
        
        public void AddCoOwner(Player p)
        {
            if (!Coowners.Contains(p))
                Coowners.Add(p);
            UpdateText();
        }
        public void RemoveCoOwner(Player p)
        {
            if (Coowners.Contains(p))
                Coowners.Remove(p);
            UpdateText();
        }
        public bool HasPermission(Player p )
        {
            return Owner == p || Coowners.Contains(p);
        }
        public void AddTeam(string team)
        {
            if (!Teams.Contains(team))
                Teams.Add(team);
            UpdateText();
        }
        public void RemoveTeam(string team)
        {
            if (Teams.Contains(team))
                Teams.Remove(team);
            UpdateText();
        }

        public void Purchase(Player p)
        {
            if (Owned) { return; }
            if (p.GetMoney() < Price)
            {
                p.Notify("<color=red>You can't afford this door!</color>");
                return;
            }
            if (Entities.Door.GetOwnedDoors(p).Count >= SCPRP.Singleton.Config.DoorsConfig.MaxDoors)
            {
                p.Notify("<color=red>Reached max amount of doors!</color>");
                return;
            }
            p.AddMoney(-Price);
            SetOwner(p);
            p.Notify($"<color=green>Bought door for ${Price}</color>");
        }

        public void Sell()
        {
            var owner = Owner;
            SetOwner(null);

            owner.AddMoney((int)(Price * 0.85));
            owner.Notify($"<color=green>Sold door for ${(int)(Price * 0.85)}</color>");
        }
        public void TextVisible(bool visible)
        {
            foreach (var t in TextScreens)
            {
                if (visible)
                    t.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
                else
                    t.Scale = new UnityEngine.Vector3(0, 0, 0);
            }
        }
    }

    public class Door : BaseModule
    {
        public Dictionary<LabApi.Features.Wrappers.Door, RPDoor> Doors = new Dictionary<LabApi.Features.Wrappers.Door, RPDoor>();

        public static RPDoor GetRPDoor(LabApi.Features.Wrappers.Door door)
        {
            if (Singleton.Doors.ContainsKey(door)) return Singleton.Doors[door];
            return null;
        }


        public static List<RPDoor> GetOwnedDoors(Player p)
        {
            return Singleton.Doors.Values.Where((x) => { return x.Owner == p; }).ToList();
        }

        public static void SellAll(Player p)
        {
            foreach (var d in GetOwnedDoors(p))
                d.Sell();
        }

        public static Door Singleton;
        public override void Load()
        {
            Singleton = this;
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += MapGenerated;
            LabApi.Events.Handlers.PlayerEvents.Left += PlayerLeft;
            LabApi.Events.Handlers.ServerEvents.DoorDamaged += DoorDamaged;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += KeyPressedBuyDoor;
        }
        public override void Unload()
        {
            LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= MapGenerated;
            LabApi.Events.Handlers.PlayerEvents.Left -= PlayerLeft;
            LabApi.Events.Handlers.ServerEvents.DoorDamaged -= DoorDamaged;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= KeyPressedBuyDoor;
            Doors.Clear();
        }

        void PlayerLeft(PlayerLeftEventArgs e)
        {
            foreach (var d in GetOwnedDoors(e.Player))
                d.SetOwner(null);
        }

        void DoorDamaged(DoorDamagedEventArgs e)
        {
            var door = GetRPDoor(e.Door);
            if (door == null) return;

            if (!(e.Door.Base is IDamageableDoor)) return;

            var dmgdoor = (IDamageableDoor)e.Door.Base;

            if ((dmgdoor.IsDestroyed || dmgdoor.RemainingHealth <= 0))
            {
                door.nextRepair = DateTime.Now.AddSeconds(180);
            }
        }
        
        void KeyPressedBuyDoor(ReferenceHub hub, ServerSpecificSettingBase b)
        {
            if (b.SettingId != (int)Input.InputIds.BuyDoor) return;


            SSKeybindSetting keybind = (SSKeybindSetting)b;

            if (!keybind.SyncIsPressed) return;

            var p = Player.Get(hub);
            if (p == null) return;

            var door = p.GetLookingDoor();
            if (door == null) return;
            var rpdoor = GetRPDoor(door);
            if (rpdoor == null) return;
            if (rpdoor.Owner == p) { rpdoor.Sell(); return; }
            rpdoor.Purchase(p);

        }
        void MapGenerated()
        {
            foreach (var d in LabApi.Features.Wrappers.Door.List.Where((x) => { return !(x is ElevatorDoor) && !(x is CheckpointDoor) && x.CanInteract; }))
                    Doors.Add(d, new RPDoor(d));
        }

        public override void Tick()
        {
            foreach(var d in Doors)
            {
                if (!(d.Key.Base is IDamageableDoor)) continue;
                if (DateTime.Now < d.Value.nextRepair) continue;
                var door = (d.Key.Base as IDamageableDoor);
                if (!door.IsDestroyed) continue;
                door.ServerRepair();
            }
        }




    }
}
