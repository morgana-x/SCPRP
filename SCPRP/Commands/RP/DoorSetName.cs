using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
namespace SCPRP.Commands.RP
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoorSetName : ParentCommand, ICommand
    {
        public override string Command { get; } = "doorsetname";
        public override string Description { get; } = "Sets the name of the door";

        public override string[] Aliases { get; } = new string[] { "doorname", "namedoor", "doorlabel", "labeldoor" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {


            var p = Player.Get(sender);

          
            var door = p.GetLookingDoor();

            if (door == null)
            {
                response = $"You need to be looking at a door you own!";
                return false;
            }

            var rpdoor = Modules.Entities.Door.GetRPDoor(door);
            if (rpdoor == null || rpdoor.Owner != p)
            {
                response = $"You don't own this door!";
                return false;
            }

            if (args.Count < 1)
            {
                rpdoor.Name = string.Empty;
                response = "Reset the door's name";
                return false;
            }


            var text = string.Join(" ", args.ToArray()).Replace("<size=","").Replace("<size", "").Replace("</size>", "");//.Replace("<color ", "").Replace("</color>","");
            text = text.Trim();

            if (text.Length > 25)
            {
                response = "Door name needs to be less than 25 characters!";
                return false;
            }

            rpdoor.Name = text;
            response = $"Set door's name to {text}!";
            return true;
        }
    }
}
