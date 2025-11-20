using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace SCPRP.Commands.Admin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceSellAllDoors : ParentCommand, ICommand
    {
        public override string Command { get; } = "selldoors";
        public override string Description { get; } = "sells all of the player's doors";

        public override string[] Aliases { get; } = new string[] { "sellalldoors" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing argumenst! Correct Usage: selldoors name";
                return false;
            }

            var pl = Player.GetByNickname(args.ElementAt(0), true);
            if (pl == null)
            {
                response = $"Couldn't find player {args.ElementAt(0)}!";
                return false;
            }

            Modules.Entities.Door.SellAll(pl);
            response = $"Force sold {pl.DisplayName}'s doors!!";
            return true;
        }
    }
}
