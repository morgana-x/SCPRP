using System;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SellAllDoors : ParentCommand, ICommand
    {
        public override string Command { get; } = "selldoors";
        public override string Description { get; } = "sells all doors you own";

        public override string[] Aliases { get; } = new string[] {  };
        public override void LoadGeneratedCommands() { }
  
        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

         

            var p = Player.Get(sender);
            
            Modules.Entity.Door.SellAll(p);

            response = $"Sold all doors!";
            return true;
        }
    }
}
