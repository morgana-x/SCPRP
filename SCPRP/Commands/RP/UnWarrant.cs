using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;


namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class UnWarrant : ParentCommand, ICommand
    {
        public override string Command { get; } = "unwarrant";
        public override string Description { get; } = ".unwarrant name";

        public override string[] Aliases { get; } = new string[] { "unwarranted" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            var p = Player.Get(sender);

            if (!Modules.Players.Jobs.Government.IsGovernment(p))
            {
                response = $"Only government roles can revoke warrants";
                return false;
            }

            if (args.Count < 1)
            {
                response = "Missing arguments! Correct Usage: .warrant playername";
                return false;
            }

            var target = Player.GetByDisplayName(args.ElementAt(0));

            if (target == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(0)}\"";
                return false;
            }

            if (!Modules.Players.Jobs.Government.IsWarranted(target))
            {
                response = $"{target.DisplayName} is not currently warranted!";
                return false;
            }
            Modules.Players.Jobs.Government.Unwarrant(target);
            response = $"{target.DisplayName} is no longer warranted!";
            return true;
        }
    }
}
