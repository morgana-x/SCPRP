using InventorySystem;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPRP.Entities
{
    public class spawned_shipment : BaseEntity
    {
        public override string Name => "Spawned Shipment";

        public ItemType ItemType = ItemType.None;


        public int Amount = 0;
        public int Max = -1;

        TextToy t1;
        TextToy t2;
        public override void OnCreate(Vector3 position, Quaternion rotation)
        {
            SetItem(ItemType.Coal, -1, -1, position, rotation);
        }

        public void SetItem(ItemType item, int amount, int max, Vector3 position, Quaternion rotation)
        {

            Amount = amount; 
            Max = max;
            ItemType = item;

            InteractablePickup = Pickup.Create(item, position, rotation);
            CoreObject = InteractablePickup.GameObject;
            t1 = TextToy.Create(new UnityEngine.Vector3(0.15f, 0, 0), Quaternion.Euler(0,-90,0), InteractablePickup.Transform, false);
            t2 = TextToy.Create(new UnityEngine.Vector3(-0.15f, 0, 0), Quaternion.Euler(0, 90, 0), InteractablePickup.Transform, false);

            t1.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            t2.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);

            UpdateText();

            this.InteractablePickup.Spawn();
            t1.Spawn();
            t2.Spawn();

        }
        private void UpdateText()
        {
            t1.TextFormat = $"{Amount} / {Max}";
            t2.TextFormat = $"{Amount} / {Max}";
        }
        public override void OnDamage(Player player, int amount)
        {

        }

        public override void OnDestroy()
        {
           
        }

        public override void OnInteract(Player player)
        {
            if (Max == -1) return;
            Amount -= 1;

          
            var pickup = Pickup.Create(ItemType, CoreObject.transform.position + Vector3.up);
            pickup.Spawn();

            if (Amount == 0)
                Destroy();
            else
                UpdateText();
        }

        public override void OnTick()
        {
           
        }
    }
}
