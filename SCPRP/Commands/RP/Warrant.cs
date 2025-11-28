using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;


namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Warrant : ParentCommand, ICommand
    {
        public override string Command { get; } = "warrant";
        public override string Description { get; } = ".warrant name reason";

        public override string[] Aliases { get; } = new string[] { "warranted" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            var p = Player.Get(sender);

            if (!Modules.Players.Jobs.Government.IsGovernment(p))
            {
                response = $"Only government roles can warrant people!";
                return false;
            }

            if (args.Count < 2)
            {
                response = "Missing arguments! Correct Usage: .warrant playername reason";
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


            reason = reason.Replace("</color>", "").Replace("<color", "").Replace("<size", "").Replace("</size>", "");
            if (reason.Length > 32)
            {
                response = "Warrant reason too long! 32 character limit!";
                return false;
            }

            response = $"{target.DisplayName} is now warrant! Reason: {reason}";
            Modules.Players.Jobs.Government.SetWarranted(target, reason, p);
            return true;
        }
    }
}
