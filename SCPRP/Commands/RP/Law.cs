using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Modules.Players.Jobs;


namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Law : ParentCommand, ICommand
    {
        public override string Command { get; } = "law";
        public override string Description { get; } = ".law add/remove/reset law/index)";

        public override string[] Aliases { get; } = new string[] { };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            var p = Player.Get(sender);

            if (args.Count == 0)
            {
                response = "The Laws:\n";

                for (int i = 0; i < Mayor.Laws.Count; i++)
                    response += ($"{i}. {Mayor.Laws[i]}\n");
                return true;
            }

            if (!Mayor.IsMayor(p))
            {
                response = "Only the mayor can change the laws!";
                return false;
            }

            switch(args.First())
            {
                case "add":
                    if (args.Count < 2)
                    {
                        response = "Missing the law you want to add!";
                        return false;
                    }
                    string law = string.Join(" ", args.Skip(1));
                    response = $"Adding law \"{law}\"...";
                    return Mayor.TryAddLaw(p, law);
                case "remove":
                    if (args.Count < 2)
                    {
                        response = "Missing argument index!";
                        return false;
                    }
                    if (!int.TryParse(args.At(1), out int i))
                    {
                        response = $"Couldn't parse \"{args.At(1)}\" as an int!";
                        return false;
                    }
                    response = "Removing law...";
                    return Mayor.TryRemoveLaw(p, i);
                case "reset":
                    response = "Reset laws!";
                    return Mayor.TryResetLaws(p);
               default:
                    response = "Invalid option!";
                    return false;
            }
        }
    }
}
