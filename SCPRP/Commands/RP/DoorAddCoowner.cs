using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;

namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoorAddCoowner : ParentCommand, ICommand
    {
        public override string Command { get; } = "addcoowner";
        public override string Description { get; } = "Adds player as a coowner to the door you own";

        public override string[] Aliases { get; } = new string[] { "addowner", "dooraddplayer", "doorgrant" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing arguments! Correct Usage: addowner playername";
                return false;
            }


            var p = Player.Get(sender);

            var coowner = Player.GetByDisplayName(args.ElementAt(0));

            if (coowner == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(0)}\"";
                return false;
            }

            if (coowner == p)
            {
                response = $"Can't add yourself as a coowner!";
                return false;
            }

            var door = p.GetLookingDoor();

            if (door == null)
            {
                response = $"You need to be looking at a door you own!";
                return false;
            }

            var rpdoor = Modules.Entities.Door.GetRPDoor(door);
            if (rpdoor == null || rpdoor.Owner != p)
            {
                response = $"You don't own this door!";
                return false;
            }

            if (rpdoor.Coowners.Contains(coowner))
            {
                rpdoor.RemoveCoOwner(coowner);
                response = $"Removed {coowner.DisplayName} from door!";
                return true;
            }

            rpdoor.AddCoOwner(coowner);
            response = $"Added {coowner.DisplayName} to door!";
            return true;
        }
    }
}
