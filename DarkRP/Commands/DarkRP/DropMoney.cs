using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using DarkRP.Extensions;
using DarkRP.Modules.Entities;
using DarkRP.Modules.DB;

namespace DarkRP.Commands.DarkRP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DropMoney : ParentCommand, ICommand
    {
        public override string Command { get; } = "dropmoney";
        public override string Description { get; } = "drops money";

        public override string[] Aliases { get; } = new string[] { "dropcash" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing arguments! Correct Usage: .dropmoney amount";
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
