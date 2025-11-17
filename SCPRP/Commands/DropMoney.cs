using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entity;
namespace SCPRP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DropMoney : ParentCommand, ICommand
    {
        public override string Command { get; } = "dropmoney";
        public override string Description { get; } = "drops money";

        public override string[] Aliases { get; } = new string[] { "dropcash" };
        public override void LoadGeneratedCommands() { }
        public static Dictionary<string, DateTime> Cooldowns = new Dictionary<string, DateTime>();

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing argumenst! Correct Usage: dropmoney amount";
                return false;
            }

            long amount = 0;
            long.TryParse(args.ElementAt(0), out amount);

            if (amount <= 0)
            {
                response = $"Amount needs to be greater than zero!";
                return false;
            }

            var p = Player.Get(sender);
            if (amount > p.GetMoney())
            {
                response = $"You can't afford to drop this!";
                return false;
            }

            response = $"Dropped ${amount}";
            Money.DropMoney(p, amount);
            return true;
        }
    }
}
