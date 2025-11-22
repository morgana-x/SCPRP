using System;
using CommandSystem;
using SCPRP.Modules.Players;

namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class JobList : ParentCommand, ICommand
    {
        public override string Command { get; } = "jobs";
        public override string Description { get; } = "Lists all jobs";

        public override string[] Aliases { get; } = new string[] { "listjobs", "viewjobs" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {


            response = "Do <color=yellow>.job jobid</color> to become a job!\nJobs:\n";
            foreach(var pair in SCPRP.Singleton.Config.JobConfig.Jobs)
            {
                var maxsymbol = pair.Value.MaxPlayers != 0 ? pair.Value.MaxPlayers.ToString() : "∞";
                response += $" - <color={pair.Value.HexColour()}>{pair.Key}</color> = <color={pair.Value.HexColour()}>{pair.Value.Name}</color> ({Job.GetJobPlayers(pair.Key).Count}/{maxsymbol})<color=#555555> - {pair.Value.Description}</color>\n";
                //response += $"          - {pair.Value.Description}\n";
            }
            response += "Do <color=yellow>.job jobid</color> to become a job!\n";
            return true;
        }
    }
}
