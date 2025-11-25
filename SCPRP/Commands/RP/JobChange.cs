using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;
using SCPRP.Modules.Players;

namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class JobChange : ParentCommand, ICommand
    {
        public override string Command { get; } = "job";
        public override string Description { get; } = "Changes job";

        public override string[] Aliases { get; } = new string[] { "setjob", "changejob" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing arguments! Correct Usage: job <color=yellow>jobid</color>";
                return false;
            }


            return Job.TryChangeJob(Player.Get(sender), args.First(), out response); ;
        }
    }
}
