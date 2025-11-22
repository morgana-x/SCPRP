using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;
namespace SCPRP.Commands.Admin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Door : ParentCommand, ICommand
    {
        public override string Command { get; } = "door";
        public override string Description { get; } = "door help";

        public override string[] Aliases { get; } = new string[] { };
        public override void LoadGeneratedCommands() { }


        public bool DoorHelp(ArraySegment<string> args, Player p, out string response)
        {
            response = "Door Commands:\n";
            response += " - <color=yellow>door name </color><color=#2be1fb>text</color> - Sets the door's name\n";
            response += " - <color=yellow>door team </color><color=#2be1fb>team</color> - Sets the door's team\n";
            response += " - <color=yellow>door unown     - Unowns the door\n";
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

        public bool DoorUnown(RPDoor rpdoor, ArraySegment<string> args, Player p, out string response)
        {
            rpdoor.SetOwner(null);
            rpdoor.Teams.Clear();
            response = $"Unowned door!";
            return true;
        }

        public bool DoorSetTeam( RPDoor door, ArraySegment<string> args, Player p, out string response)
        {
            if (args.Count == 0)
            {
                response = $"Missing team!";
                return false;
            }
            string team = args.First();
            if (!Modules.Players.Job.IsValidTeam(team))
            {
                response = $"Invalid team!";
                return false;
            }
            door.SetOwner(null);
            door.AddTeam(team);
            response = $"Set team to {Modules.Players.Job.GetColouredTeamName(team)}!";
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


            var door = p.GetLookingDoor();

            if (door == null)
            {
                response = $"You need to be looking at a door!";
                return false;
            }

            var rpdoor = Modules.Entities.Door.GetRPDoor(door);

            var argsShortend = args.Segment(1);
            switch (cmd)
            {
                case "name":
                    return DoorSetName(rpdoor, argsShortend, p, out response);
                case "unown":
                    return DoorUnown(rpdoor, argsShortend, p, out response);
                case "team":
                    return DoorSetTeam(rpdoor, argsShortend, p, out response);
                default:
                    bool result = DoorHelp(args, p, out response);
                    response = $"<color=red>Incorrect Sub Command </color><color=yellow>\"{cmd}\"</color><color=red>.\nValid Sub Commands:</color>\n{response}";
                    return result;
            }

        }
    }
}
