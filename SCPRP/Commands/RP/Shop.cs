using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Players;

namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Shop : ParentCommand, ICommand
    {
        public override string Command { get; } = "shop";
        public override string Description { get; } = ".shop buy item, .shop list";

        public override string[] Aliases { get; } = new string[] { "store" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing argument! Correct Usage: .shop buy item or .shop list";
                return false;
            }
            var p = Player.Get(sender);
            if (args.First() == "list")
            {
                response = "\n====== Items ======\n";
                foreach (var i in Modules.Players.Shop.GetAvaiableItems(p))
                    response += $"= {i.Entity} ${i.Price} ({Entity.Singleton.GetEntities(p, i.Entity).Count}/{i.Max})\n";
                response += "==================";
                return true;
            }

            if (args.Count < 2)
            {
                response = "Missing argument! Correct Usage: .shop buy item";
                return false;
            }
            var itemid = args.ElementAt(1);

            var item = Modules.Players.Shop.GetItem(itemid);
            if (item == null)
            {
                response = $"Invalid entity to purchase!";
                return false;
            }

            if (!item.CanPurchase(p))
            {
                response = $"Cannot purchase this item! Incorrect Job/Rank!";
                return false;
            }
            if (Entity.Singleton.GetEntities(p, item.Entity).Count >= item.Max)
            {
                response = $"Reached limit of {item.Max} {item.Entity}s!";
                return false;
            }
            if (item.Price > p.GetMoney())
            {
                response = $"You can't afford to drop this!";
                return false;
            }

            response = $"Purchased {item.Entity} for ${item.Price}!";
            Modules.Players.Shop.BuyItem(p, item);
            return true;
        }
    }
}
