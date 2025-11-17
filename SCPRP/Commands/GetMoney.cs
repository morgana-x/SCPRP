using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
namespace SCPRP.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GetMoney : ParentCommand, ICommand
    {
        public override string Command { get; } = "getmoney";
        public override string Description { get; } = "gets player money";

        public override string[] Aliases { get; } = new string[] { "getcash" };
        public override void LoadGeneratedCommands() { }
        public static Dictionary<string, DateTime> Cooldowns = new Dictionary<string, DateTime>();

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing argumenst! Correct Usage: getmoney name";
                return false;
            }

            var pl = Player.GetByNickname(args.ElementAt(0), true);
            if (pl == null)
            {
                response = $"Couldn't find player {args.ElementAt(0)}!";
                return false;
            }

            response = $"{args.ElementAt(0)} has ${pl.GetMoney()}!";
            return true;
        }
    }
}
