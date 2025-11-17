using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using SCPRP.Extensions;
namespace SCPRP.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetMoney : ParentCommand, ICommand
    {
        public override string Command { get; } = "setmoney";
        public override string Description { get; } = "sets player money";

        public override string[] Aliases { get; } = new string[] { "setcash" };
        public override void LoadGeneratedCommands() { }
        public static Dictionary<string, DateTime> Cooldowns = new Dictionary<string, DateTime>();

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 2)
            {
                response = "Missing argumenst! Correct Usage: setmoney name amount";
                return false;
            }

            var pl = LabApi.Features.Wrappers.Player.GetByNickname(args.ElementAt(0), true);
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

            pl.SetMoney(amount);

            response = $"Set {args.ElementAt(0)}'s cash to {args.ElementAt(1)}!";
            return true;
        }
    }
}
