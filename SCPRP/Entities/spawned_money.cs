using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Players;
using UnityEngine;

namespace SCPRP.Entities
{
    public class spawned_money : BaseEntity
    {
        public override float MaxHealth => 1;

        private long _amount = 0;
        public long Amount { get { return _amount; } set { _amount = value; } }

        TextToy t1;
        TextToy t2;
        public override void OnCreate(Vector3 Position)
        {
            this.InteractablePickup = Pickup.Create(ItemType.KeycardJanitor, Position);
            this.CoreObject = this.InteractablePickup.GameObject;



            t1 = TextToy.Create(new UnityEngine.Vector3(0, 0.015f, 0), new UnityEngine.Quaternion(0.7071068f, 0, 0, 0.7071068f), InteractablePickup.Transform, false);
            t2 = TextToy.Create(new UnityEngine.Vector3(0, -0.015f, 0), new UnityEngine.Quaternion(-0.7071068f, 0, 0, 0.7071068f), InteractablePickup.Transform, false);

            t1.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            t2.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);

            UpdateText();

            this.InteractablePickup.Spawn();
            t1.Spawn();
            t2.Spawn();
        }

        private void UpdateText()
        {
            t1.TextFormat = $"<color #55ff55>${_amount}</color>";
            t2.TextFormat = $"<color #55ff55>${_amount}</color>";
        }

        public void SetAmount(long amount)
        {
            Amount = amount;
        }

        public override void OnDamage(Player player, int amount)
        {
            Health -= amount;
            if (Health <= 0) { Destroy(); }
        }

        public override void OnDestroy()
        {
            
        }

        public override void OnInteract(Player player)
        {
            HUD.ShowHint(player, $"<color=green>Picked up ${_amount}</color>");
            player.AddMoney(_amount);
            _amount = 0;
            Destroy();
        }

        public override void OnTick()
        {
            
        }
    }
}
