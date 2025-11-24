using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;
using SCPRP.Modules.Players.Jobs;


namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Hit : ParentCommand, ICommand
    {
        public override string Command { get; } = "hit";
        public override string Description { get; } = ".hit name amount";

        public override string[] Aliases { get; } = new string[] { "bounty" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (args.Count < 2)
            {
                response = "Missing arguments! Correct Usage: .hit playername amount";
                return false;
            }

            var target = Player.GetByDisplayName(args.ElementAt(0));

            if (target == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(0)}\"";
                return false;
            }

            int amount = 0;
            int.TryParse(args.ElementAt(1), out amount);

            if (amount <= 0)
            {
                response = $"Amount needs to be greater than zero!";
                return false;
            }
            if (amount < Hitman.Singleton.Config.MinimumPrice)
            {
                response = $"Amount needs to be greater than ${Hitman.Singleton.Config.MinimumPrice}!";
                return false;
            }

            var p = Player.Get(sender);
            if (amount > p.GetMoney())
            {
                response = $"You can't afford to place this hit!";
                return false;
            }
            if (Modules.Players.Jobs.Hitman.IsHitman(p))
            {
                response = $"Hitmen cannot place hits!";
                return false;
            }
            if (p == target)
            {
                response = $"Can't place a hit on yourself!";
                return false;
            }
            response = $"Placed a ${amount} hit on {target.DisplayName}!";
            p.AddMoney(-amount);
            Modules.Players.Jobs.Hitman.AddHit(target, amount);
            return true;
        }
    }
}
