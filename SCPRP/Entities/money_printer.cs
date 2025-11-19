using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using System;
using UnityEngine;

namespace SCPRP.Entities
{
    public class money_printer : BaseEntity
    {
        public override float MaxHealth => 50;

        DateTime nextPrint = DateTime.Now;

        private long _amount;

        public long IncreaseRate = 30;
        public int Rate = 30;
        public long Amount { get {  return _amount; } set { _amount = value; UpdateText(); } }


        TextToy amountScreen;
        public override void OnCreate(Vector3 position)
        {
    
            var primitive = PrimitiveObjectToy.Create(position, networkSpawn:true);
            primitive.Type = PrimitiveType.Cube;
            primitive.Scale = new Vector3(0.6f, 0.2f, 0.6f);
            primitive.Color = Color.white;

            primitive.Base.gameObject.AddComponent<Rigidbody>();

            var interactable = InteractableToy.Create(new Vector3(0,0,0), primitive.Base.gameObject.transform, networkSpawn:true);
            interactable.Scale = primitive.Scale * 1.1f;


            amountScreen = TextToy.Create(new Vector3(0, 0.25f, -0.1f), Quaternion.Euler(90, 0, 0), primitive.Base.gameObject.transform, true);
            amountScreen.Scale = new Vector3(0.2f, 0.2f, 0.2f);


            CoreObject = primitive.GameObject;
            Interactable = interactable;

            primitive.Spawn();
            interactable.Spawn();
            amountScreen.Spawn();

            UpdateText();
            nextPrint = DateTime.Now.AddSeconds(Rate);
        }

        public override void OnDamage(Player player, int amount)
        {
            
        }

        public override void OnDestroy()
        {
            
        }

        public override void OnInteract(Player player)
        {
            
        }

        void UpdateText()
        {
            amountScreen.TextFormat = $"<color #55ff55>${_amount}</color>";
        }

        public override void OnTick()
        {
            if (DateTime.Now < nextPrint) return;
            Amount += IncreaseRate;
            nextPrint = DateTime.Now.AddSeconds(Rate);
        }
    }
}
