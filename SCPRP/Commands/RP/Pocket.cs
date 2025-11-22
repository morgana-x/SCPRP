using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Entities;

namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Pocket : ParentCommand, ICommand
    {
        public override string Command { get; } = "pocket";
        public override string Description { get; } = "Stores items";

        public override string[] Aliases { get; } = new string[] { "bag" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {

            if (args.Count < 1)
            {
                response = "Missing argument! Correct Usage: .pocket grab & .pocket drop";
                return false;
            }

            response = "Incorrect argument!";

            var p = Player.Get(sender);
            if (p == null) 
                return false;

     

            switch (args.ElementAt(0))
            {
                case "grab":
                    if (Modules.Players.Pocket.GetInventory(p).Count >= SCPRP.Singleton.Config.PocketConfig.MaxCapacity)
                    {
                        response = $"Reached limit of {SCPRP.Singleton.Config.PocketConfig.MaxCapacity} in pocket!";
                        return false;
                    }
                    var entity = p.GetLookingEntity();

                    if (entity == null)
                    {
                        response = "Need to be looking at an entity";
                        return false;
                    }
                    Modules.Players.Pocket.Pickup(p, entity);
                    response = "Picking up entity...";
                    break;
                case "drop":
                    if (Modules.Players.Pocket.GetInventory(p).Count == 0)
                    {
                        response = "There are no entities inside your pocket!";
                        return false;
                    }
                    Modules.Players.Pocket.Drop(p);
                    response = "Dropping entity...";
                    break;
            }


            return true;
        }
    }
}
