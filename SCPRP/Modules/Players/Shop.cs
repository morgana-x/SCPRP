
using InventorySystem;
using LabApi.Features.Wrappers;
using SCPRP.Entities;
using SCPRP.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SCPRP.Modules.Players
{
    public class ShopItem
    {
        public string Entity { get; set; }
        public int Price { get; set; }
        public int Max { get; set; } = 1;

        public bool IsShipment { get; set; } = false;
        public int ShipmentAmount { get; set; } = 10;

        public List<string> AllowedJobs { get; set; } = new List<string>();

        public ShopItem() { }
        public ShopItem(string entity, int price, int max)
        {
            Entity = entity;
            Price = price;
            Max = max;
        }

    
        public bool CanPurchase(Player pl)
        { 
            return AllowedJobs.IsEmpty() || AllowedJobs.Contains(Job.GetJob(pl));
        }
    }

    public class ShipItem : ShopItem
    {
        public ShipItem(ItemType type, int price, List<string> allowedJobs, int shipamount = 5)
        {
            Price = price;
            Entity = System.Enum.GetName(typeof(ItemType), type);
            ShipmentAmount = shipamount;
            AllowedJobs = allowedJobs;
            IsShipment = true;
        }
        public ShipItem(ItemType type, int price, int shipamount = 5)
        {

            Price = price;
            Entity = System.Enum.GetName(typeof(ItemType), type);
            ShipmentAmount = shipamount;
            IsShipment = true;
        }

    }


    public class ShopConfig
    {
        public int MaxShipments { get; set; } = 10;
        public List<ShopItem> Items { get; set; } = new List<ShopItem>()
        {
            new ShopItem("money_printer", 2000, 3),

            new ShipItem(ItemType.Ammo12gauge, 1000, 1),
            new ShipItem(ItemType.Ammo44cal, 1000, 1),
            new ShipItem(ItemType.Ammo556x45, 1000, 1),
            new ShipItem(ItemType.Ammo762x39, 1000, 1),
            new ShipItem(ItemType.Ammo9x19, 1000, 1),

            new ShipItem(ItemType.Medkit, 6000, new List<string>(){"medic" }, shipamount:3  ),
            new ShipItem(ItemType.Painkillers, 2000, new List<string>(){"medic" }, shipamount:4  ),

            new ShipItem(ItemType.GrenadeHE, 15000, new List<string>(){"gundealer" }, shipamount:3 ),
            new ShipItem(ItemType.GrenadeFlash, 10000, new List<string>(){"gundealer" }, shipamount:3 ),

            new ShipItem(ItemType.GunAK, 4000, new List<string>(){"gundealer" } ,shipamount:1 ),
            new ShipItem(ItemType.GunCrossvec, 3000, new List<string>(){"gundealer" } , shipamount:1 ),

            new ShipItem(ItemType.GunCOM18, 1100, new List<string>(){"gundealer" }  , shipamount : 3),
            new ShipItem(ItemType.GunCOM15, 1000, new List<string>(){"gundealer" }  , shipamount : 3),

            new ShipItem(ItemType.GunShotgun, 3500, new List<string>(){"gundealer" } ,shipamount : 1 ),
            new ShipItem(ItemType.GunRevolver, 1900, new List<string>(){"gundealer" } ,shipamount : 1 ),

            new ShipItem(ItemType.GunE11SR, 5000, new List<string>(){"gundealer" } ,shipamount : 1 ),
            new ShipItem(ItemType.GunFRMG0, 2000, new List<string>(){"gundealer" } ,shipamount : 1 ),
            new ShipItem(ItemType.GunFSP9, 1500, new List<string>(){"gundealer" } ,shipamount : 1 ),

            new ShipItem(ItemType.ArmorLight, 4000, new List<string>(){"gundealer" }, shipamount:1  ),
            new ShipItem(ItemType.ArmorCombat, 6000, new List<string>(){"gundealer" }, shipamount:1  ),
            new ShipItem(ItemType.ArmorHeavy, 8000, new List<string>(){"gundealer" }, shipamount:1  ),


            new ShipItem(ItemType.KeycardJanitor, 100, new List<string>(){"keycard" }, shipamount:1  ),
            new ShipItem(ItemType.KeycardScientist, 10000, new List<string>(){"keycard" }, shipamount:1  ),
            new ShipItem(ItemType.KeycardResearchCoordinator, 15000, new List<string>(){"keycard" }, shipamount:1  ),
            new ShipItem(ItemType.KeycardMTFOperative, 40000, new List<string>(){"keycard" }, shipamount:1  ),
            new ShipItem(ItemType.KeycardO5, 80000, new List<string>(){"keycard" }, shipamount:1  ),
        };

    }

    public class Shop : BaseModule
    {
        public static Shop Singleton;

        public static ShopConfig shopConfig {  get { return SCPRP.Singleton.Config.ShopConfig; } }
        public override void Load()
        {
            Singleton = this;

        }

        public override void Unload()
        {
            
        }

        public static List<ShopItem> GetAvaiableItems(Player pl)
        {
            return SCPRP.Singleton.Config.ShopConfig.Items.Where((x) => { return x.CanPurchase(pl); }).ToList();
        }

        public static List<ShopItem> GetItems()
        {
            return shopConfig.Items;
        }

        public static ShopItem GetItem(string id)
        {
            var result = shopConfig.Items.Where((x) => { return x.Entity == id; }).ToList();
            if (result.Count == 0) return null;
            return result.First();
        }

        
        public static void SpawnShipment(Player p, ShopItem item)
        {
            if (!System.Enum.TryParse(item.Entity, out ItemType gameItem))
            {
                HUD.ShowHint(p, $"<color #ff5555>ERROR SHIPMENT ITEM WAS INVALID</color>");
                return;
            }

            if (item.ShipmentAmount == 1)
            {
                if (System.Enum.GetName(typeof(ItemType), gameItem).StartsWith("Ammo"))
                {
                    p.Inventory.ServerAddAmmo(gameItem, 50);
                    // pickup = AmmoPickup.Create(gameItem, p.Camera.position + p.Camera.forward * 0.5f);
                    //((AmmoPickup)pickup).Ammo = 100;
                    return;
                }

                Pickup pickup = Pickup.Create(gameItem, p.Camera.position + p.Camera.forward * 0.5f);
                pickup.Spawn();
                return;
            }

            var ship = (spawned_shipment)Entity.Singleton.CreateEntity("spawned_shipment");
            ship.Owner = p;
            ship.SetItem(gameItem, item.ShipmentAmount, item.ShipmentAmount, p.Camera.position + p.Camera.forward * 0.5f, UnityEngine.Quaternion.Euler(0,0,0));
            Entity.Singleton.AddSpawnedEntity(ship);
        }

        public static void BuyItem(Player p, ShopItem item)
        {
            if (!item.CanPurchase(p))
            {
                HUD.ShowHint(p, $"<color #ff5555>Not allowed to purchase this entity! Wrong job/rank!</color>");
                return;
            }
            if (p.GetMoney() < item.Price)
            {
                HUD.ShowHint(p, $"<color #ff5555>Cannot afford this entity!</color>");
                return;
            }
            if (!item.IsShipment && Entity.Singleton.GetEntities(p, item.Entity).Count >= item.Max)
            {
                HUD.ShowHint(p, $"<color #ff5555>Reached limit of {item.Max} {item.Entity}s!</color>");
                return;
            }
            else if (item.IsShipment && item.ShipmentAmount > 1 && Entity.Singleton.GetEntities(p, "spawned_shipment").Count >= shopConfig.MaxShipments)
            {
                HUD.ShowHint(p, $"<color #ff5555>Reached limit of {shopConfig.MaxShipments} spawned_shipments!</color>");
                return;
            }

            p.AddMoney(-item.Price);
            HUD.ShowHint(p, $"<color #55ff55>Purchased {item.Entity} for ${item.Price}!</color>");

            if (item.IsShipment)
                SpawnShipment(p, item);
            else
                Entity.Singleton.SpawnEntity(item.Entity, p.Camera.position + p.Camera.forward*0.85f, p);
        }

        public override void Tick()
        {
            
        }
    }
}
