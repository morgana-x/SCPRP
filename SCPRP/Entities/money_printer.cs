using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Players;
using System;
using UnityEngine;
using Utils;

namespace SCPRP.Entities
{
    public class money_printer : BaseEntity
    {
        public class MoneyPrinterConfig
        {
            public int IncreaseAmount { get; set; } = 50;
            public int Rate { get; set; } = 30;

            public int MoneyLimit { get; set; } = 20000;
        }

        public override string Name => "Money Printer";
        public override float MaxHealth => 50;

        DateTime nextPrint = DateTime.Now;

        private long _amount;

        public long IncreaseAmount = 50;
        public int Rate = 30;
        public long Amount { get {  return _amount; } set { _amount = value; UpdateText(); } }


        TextToy amountScreen;
        public override void OnCreate(Vector3 position, Quaternion rotation)
        {
    
            var primitive = PrimitiveObjectToy.Create(position, rotation, networkSpawn: false);
            primitive.Type = PrimitiveType.Cube;
            primitive.Scale = new Vector3(0.6f, 0.2f, 0.6f);
            primitive.Color = Color.white;
            primitive.SyncInterval = 0.1f;
            primitive.MovementSmoothing = 200;
            primitive.IsStatic = false;

            primitive.Base.gameObject.AddComponent<Rigidbody>();

            primitive.Spawn();

            var interactable = InteractableToy.Create(new Vector3(0,0,0), primitive.Transform, networkSpawn:false);
            interactable.Shape = AdminToys.InvisibleInteractableToy.ColliderShape.Box;
            interactable.Scale = new Vector3(1.1f, 1.1f, 1.1f);
            interactable.IsStatic = false;
            interactable.Spawn();

            amountScreen = TextToy.Create((primitive.Transform.up * 0.52f), Quaternion.Euler(90, 0, 0), primitive.Transform, networkSpawn: false);
            amountScreen.Scale = new Vector3(0.15f, 0.15f, 0.15f);
            amountScreen.IsStatic = false;
            amountScreen.Spawn();

            CoreObject = primitive.GameObject;
            Interactable = interactable;

 


            UpdateText();

            IncreaseAmount = SCPRP.Singleton.Config.MoneyPrinterConfig.IncreaseAmount;
            Rate = SCPRP.Singleton.Config.MoneyPrinterConfig.Rate;

            nextPrint = DateTime.Now.AddSeconds(Rate);
        }

        public override void OnDamage(Player player, int amount)
        {
            Health -= amount;
            if (Health <= 0) Destroy();
        }

        public override void OnDestroy()
        {
            ExplosionUtils.ServerSpawnEffect(CoreObject.transform.position, ItemType.GrenadeHE);
        }

        public override void OnInteract(Player player)
        {
            if (Amount <= 0) return;
            var amount = Amount;
            Amount = 0;
            player.AddMoney(amount);
            HUD.ShowHint(player, $"<color #55ff55>Picked up {amount}!</color>");
            UpdateText();
        }

        void UpdateText()
        {
            amountScreen.TextFormat = $"<color #55ff55>${_amount}</color><br>";
            if (Owner != null) 
                amountScreen.TextFormat += Owner.GetColouredName();
        }

        public override void OnTick()
        {
            if (DateTime.Now < nextPrint) return;
            if (Amount >= SCPRP.Singleton.Config.MoneyPrinterConfig.MoneyLimit) return;

            Amount += IncreaseAmount;
            if (Amount >= SCPRP.Singleton.Config.MoneyPrinterConfig.MoneyLimit)
                Amount = SCPRP.Singleton.Config.MoneyPrinterConfig.MoneyLimit;

            nextPrint = DateTime.Now.AddSeconds(Rate);
        }
    }
}
