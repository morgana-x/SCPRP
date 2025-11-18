using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;

namespace SCPRP.Commands.Admin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddMoney : ParentCommand, ICommand
    {
        public override string Command { get; } = "addmoney";
        public override string Description { get; } = "adds player money";

        public override string[] Aliases { get; } = new string[] { "addcash" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 2)
            {
                response = "Missing argumenst! Correct Usage: addmoney name amount";
                return false;
            }

            var pl = Player.GetByNickname(args.ElementAt(0), true);
            if (pl == null)
            {
                response = $"Couldn't find player {args.ElementAt(0)}!";
                return false;
            }

            int amount = 0;
            if (!int.TryParse(args.ElementAt(1), out amount))
            {
                response = $"Invalid amount {args.ElementAt(1)}!";
                return false;
            }

            pl.AddMoney(amount);

            response = $"Gave {args.ElementAt(0)} ${args.ElementAt(1)}! They now have ${pl.GetMoney()}";
            return true;
        }
    }
}
