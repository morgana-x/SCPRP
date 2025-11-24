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
    public class Wanted  : ParentCommand, ICommand
    {
        public override string Command { get; } = "want";
        public override string Description { get; } = ".want name reason";

        public override string[] Aliases { get; } = new string[] { "wanted" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            var p = Player.Get(sender);

            if (!Modules.Players.Jobs.Government.IsGovernment(p))
            {
                response = $"Only government roles can set people as wanted!";
                return false;
            }

            if (args.Count < 2)
            {
                response = "Missing arguments! Correct Usage: .want playername reason";
                return false;
            }

            var target = Player.GetByDisplayName(args.ElementAt(0));

            if (target == null)
            {
                response = $"Couldn't find player \"{args.ElementAt(0)}\"";
                return false;
            }


            string reason = string.Join(" ", args.Segment(1).ToArray());
            if (reason.Trim() == "")
                reason = "Illegal Activities";

     
       
            response = $"{target.DisplayName} is now wanted! Reason: {reason}";
            Modules.Players.Jobs.Government.SetWanted(target, reason, p);
            return true;
        }
    }
}
