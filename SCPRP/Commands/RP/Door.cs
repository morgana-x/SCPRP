using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;
namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Door : ParentCommand, ICommand
    {
        public override string Command { get; } = "door";
        public override string Description { get; } = ".door help";

        public override string[] Aliases { get; } = new string[] {  };
        public override void LoadGeneratedCommands() { }


        public bool DoorHelp(ArraySegment<string> args, Player p, out string response)
        {
            response = "Door Commands:\n";
            response += " - <color=yellow>.door name </color><color=#2be1fb>text </color>                - Sets the door's name\n";
            response += " - <color=yellow>.door coowner add    </color><color=#2be1fb>playername</color> - Adds a coowner to the door\n";
            response += " - <color=yellow>.door coowner remove </color><color=#2be1fb>playername</color> - Removes a coowner to the door\n";
            response += " - <color=yellow>.door sell</color>                      - Sells the door\n";
            response += " - <color=yellow>.door sellall</color>                   - Sells alls door you own\n";
            response += " - <color=yellow>.door buy</color>                       - Buys the door\n";
            return true;
        }
        public bool DoorSetName(RPDoor rpdoor, ArraySegment<string> args, Player p, out string response)
        {
            if (args.Count < 1)
            {
                rpdoor.Name = string.Empty;
                response = "Reset the door's name";
                return false;
            }


            var text = string.Join(" ", args.ToArray()).Replace("<size=", "").Replace("<size", "").Replace("</size>", "");//.Replace("<color ", "").Replace("</color>","");
            text = text.Trim();

            if (text.Length > 25)
            {
                response = "Door name needs to be less than 25 characters!";
                return false;
            }

            rpdoor.Name = text;
            response = $"Set door's name to {text}!";
            return true;
        }

        public bool DoorCoowner(RPDoor rpdoor, ArraySegment<string> args, Player p, out string response)
        {
            var addremove = args.ElementAt(0);

            if (addremove != "add" && addremove != "remove")
            {
                response = $"Incorrect useage! Do <color=yellow>.door coowner add playername</color> or <color=yellow>.door coowner remove playername</color>.";
                return false;
            }

            if (args.Count < 2)
            {
                response = $"<color=red>Missing playername!</color> Do <color=yellow>.door coowner {addremove} playername</color>!!";
                return false;
            }

            var coowner = Player.GetByDisplayName(args.ElementAt(1));

            if (coowner == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(1)}\"";
                return false;
            }

            if (coowner == p)
            {
                response = $"Can't add yourself as a coowner!";
                return false;
            }

            if (addremove == "add")
            {
                rpdoor.AddCoOwner(coowner);
                response = $"Added {coowner.DisplayName} to door!";
            }
            else
            {
                rpdoor.RemoveCoOwner(coowner);
                response = $"Removed {coowner.DisplayName} from door!";
            }

            return true;
        }
        public bool DoorSell(RPDoor rpdoor, ArraySegment<string> args, Player p, out string response)
        {
            rpdoor.Sell();
            response = $"Sold door!";
            return true;
        }

        public bool DoorSellAll(Player p, out string response)
        {
            Modules.Entities.Door.SellAll(p);
            response = $"Sold all doors!";
            return true;
        }
        public bool DoorBuy(Player p, RPDoor door, out string response)
        {
            if (door.Owner == p)
            {
                response = "You already own this door!";
                return false;
            }
            if (door.Owned)
            {
                response = "This door is already owned!";
                return false;
            }
            if (p.GetMoney() < door.Price)
            {
                response = "<color=red>You can't afford this door!</color>";
                return false;
            }
            if (Modules.Entities.Door.GetOwnedDoors(p).Count >= Modules.Entities.Door.Singleton.Config.MaxDoors)
            {
                response = "<color=red>Reached max amount of doors!</color>";
                return false;
            }

            door.Purchase(p);
            response = "Buying door!";
            return true;
        }
        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {


            var p = Player.Get(sender);

            if (args.Count == 0 || args.ElementAt(0) == "help")
            {
                return DoorHelp(args, p, out response);
            }
            var cmd = args.ElementAt(0);

            if (cmd== "sellall")
            {
                return DoorSellAll(p, out response);
            }
            
            var door = p.GetLookingDoor();

            if (door == null)
            {
                response = $"You need to be looking at a door!";
                return false;
            }

            var rpdoor = Modules.Entities.Door.GetRPDoor(door);

            if (rpdoor != null && cmd == "buy")
            {
                return DoorBuy(p, rpdoor, out response);
            }

            if (rpdoor == null || rpdoor.Owner != p)
            {
                response = $"You don't own this door!";
                return false;
            }

            var argsShortend = args.Segment(1);
            switch (cmd)
            {
                case "name":
                    return DoorSetName(rpdoor, argsShortend, p, out response);
                case "sell":
                    return DoorSell(rpdoor, argsShortend, p, out response);
                case "coowner":
                    return DoorCoowner(rpdoor, argsShortend, p, out response);
                default:
                    bool result = DoorHelp(args, p, out response);
                    response = $"<color=red>Incorrect Sub Command </color><color=yellow>\"{cmd}\"</color><color=red>.\nValid Sub Commands:</color>\n{response}";
                    return result;

            }

        }
    }
}
