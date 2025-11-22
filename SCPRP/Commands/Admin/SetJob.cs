using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Players;

namespace SCPRP.Commands.Admin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetJob : ParentCommand, ICommand
    {
        public override string Command { get; } = "setjob";
        public override string Description { get; } = "Force sets a player's job";

        public override string[] Aliases { get; } = new string[] { "forcejob" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 2)
            {
                response = "Missing arguments! Correct Usage: setjob name jobid";
                return false;
            }

            var pl = Player.GetByNickname(args.ElementAt(0), true);
            if (pl == null)
            {
                response = $"Couldn't find player {args.ElementAt(0)}!";
                return false;
            }

            var job = args.ElementAt(1);
            
            if (!Job.IsValidJob(job))
            {
                response = $"Couldn't find job {job}!";
                return false;
            }

            pl.SetJob(job);
            response = $"Set {pl.DisplayName}'s job to {job}";
            return true;
        }
    }
}
