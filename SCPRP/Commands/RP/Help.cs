using System;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using CommandSystem;


namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Help : ParentCommand, ICommand
    {
        public override string Command { get; } = "rphelp";
        public override string Description { get; } = "Lists commands";

        public override string[] Aliases { get; } = new string[] { "rpcommands" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            response = "\nCommands:\n";
            var classes = Assembly.GetExecutingAssembly()
                         .GetTypes()
                         .Where(t => t.IsClass && t.Namespace.StartsWith("SCPRP.Commands.RP"))
                         .ToList();

            foreach (var type in classes.Where((x) => { return x.IsSubclassOf(typeof(ParentCommand)); }))
            {
                var instance = ((ParentCommand)Activator.CreateInstance(type));
                response += "." + instance.Command + " - " + instance.Description + "\n";
            }

        
            return true;
        }
    }
}
