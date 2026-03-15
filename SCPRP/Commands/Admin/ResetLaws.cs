using System;
using CommandSystem;
using SCPRP.Modules.Players.Jobs;

namespace SCPRP.Commands.Admin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ResetLaws : ParentCommand, ICommand
    {
        public override string Command { get; } = "resetlaws";
        public override string Description { get; } = "resets the laws";

        public override string[] Aliases => new string[]{};

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            Mayor.ResetLaws();
            response = $"Reset the laws";
            return true;
        }
    }
}
