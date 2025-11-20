
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SCPRP.Modules.Players
{
    public class ShopItem
    {
        public string Entity { get; set; }
        public int Price { get; set; }
        public int Max = 1;
        public List<string> AllowedJobs = new List<string>();

        public ShopItem() { }
        public ShopItem(string entity, int price, int max)
        {
            Entity = entity;
            Price = price;
            Max = max;
        }

        public bool CanPurchase(Player pl)
        { 
            return AllowedJobs.Count == 0 || AllowedJobs.Contains(Job.GetJob(pl));
        }
    }

    public class ShopConfig
    {
        public List<ShopItem> Items { get; set; } = new List<ShopItem>()
        {
            new ShopItem("money_printer", 2000, 3)
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
            return shopConfig.Items.Where((x) => { return x.CanPurchase(pl); }).ToList();
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

        public static void BuyItem(Player p, ShopItem item)
        {
            if (Entity.Singleton.GetEntities(p, item.Entity).Count >= item.Max)
            {
                HUD.ShowHint(p, $"<color #ff5555>Reached limit of {item.Max} {item.Entity}s!</color>");
                return;
            }
            if (p.GetMoney() < item.Price)
            {
                HUD.ShowHint(p, $"<color #ff5555>Cannot afford this entity!</color>");
                return;
            }
            p.AddMoney(-item.Price);
            HUD.ShowHint(p, $"<color #55ff55>Purchased {item.Entity} for ${item.Price}!</color>");
            Entity.Singleton.SpawnEntity(item.Entity, p.Camera.position + p.Camera.forward*0.85f, p);
        }

        public override void Tick()
        {
            
        }
    }
}
