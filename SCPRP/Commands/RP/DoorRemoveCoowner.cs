using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;

namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoorRemoveCoowner : ParentCommand, ICommand
    {
        public override string Command { get; } = "removecoowner";
        public override string Description { get; } = "Removes player as a coowner from a door you own";

        public override string[] Aliases { get; } = new string[] { "removeowner", "doorremoveplayer", "doorrevoke" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing arguments! Correct Usage: removecoowner playername";
                return false;
            }


            var p = Player.Get(sender);

            var coowner = Player.GetByDisplayName(args.ElementAt(0));

            if (coowner == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(0)}\"";
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

            if (!rpdoor.Coowners.Contains(coowner))
            {
                response = $"{coowner} is not a coowner of this door!";
                return false;
            }
        
            rpdoor.RemoveCoOwner(coowner);
            response = $"Removed {coowner.DisplayName} from door!";
            return true;
        }
    }
}
