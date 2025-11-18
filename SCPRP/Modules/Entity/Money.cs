using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Players;
using System.Collections.Generic;

namespace SCPRP.Modules.Entity
{
    public class Money : BaseModule
    {
        public Dictionary<Pickup, long> MoneyPickups = new Dictionary<Pickup, long>();

        static Money Singleton;
        public override void Load()
        {
            Singleton = this;
            PlayerEvents.PickingUpItem += OnPicking;
        }

        public override void Unload()
        {
            PlayerEvents.PickingUpItem -= OnPicking;
        }

        public override void Tick()
        {

        }

        private void OnPicking(PlayerPickingUpItemEventArgs e)
        {
            if (MoneyPickups.ContainsKey(e.Pickup))
            {
                e.IsAllowed = false;
                HUD.ShowHint(e.Player, $"<color=green>Picked up ${MoneyPickups[e.Pickup]}</color>");
                e.Player.AddMoney(MoneyPickups[e.Pickup]);
                e.Pickup.Destroy();
            }
        }

        public static Pickup DropMoney(UnityEngine.Vector3 Position, long amount)
        {
            Pickup keycard = Pickup.Create(ItemType.KeycardJanitor, Position);
            (Singleton).MoneyPickups.Add(keycard, amount);



            var t1 = TextToy.Create(new UnityEngine.Vector3(0, 0.015f, 0), new UnityEngine.Quaternion(0.7071068f, 0, 0, 0.7071068f),  keycard.Transform, false);
            var t2 = TextToy.Create(new UnityEngine.Vector3(0, -0.015f, 0), new UnityEngine.Quaternion(-0.7071068f, 0, 0, 0.7071068f), keycard.Transform, false);

            t1.TextFormat = $"<color #55ff55>${amount}</color>";
            t2.TextFormat = $"<color #55ff55>${amount}</color>";
            t1.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            t2.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);

            keycard.Spawn();
            t1.Spawn();
            t2.Spawn();
            return keycard;
        }

        public static Pickup DropMoney(Player player, long amount)
        {
            if (player.GetMoney() < amount)
            {
                HUD.ShowHint(player, "<color=red>Insufficient funds!</color>");
                return null;
            }
            player.AddMoney(-amount);
            HUD.ShowHint(player, $"<color=green>Dropped ${amount}</color>");
            return DropMoney(player.Camera.position + (player.Camera.forward * 0.8f), amount);
        }

        public static long GetAmount(Pickup pickup)
        {
            if (!Singleton.MoneyPickups.ContainsKey(pickup)) return 0;
            return (Singleton).MoneyPickups[pickup];
        }
    }
}
