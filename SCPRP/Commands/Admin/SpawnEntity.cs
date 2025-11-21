using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace SCPRP.Commands.Admin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnEntity : ParentCommand, ICommand
    {
        public override string Command { get; } = "spawnentity";
        public override string Description { get; } = "spawns a rp entity";

        public override string[] Aliases { get; } = new string[] { "spawnent" };
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            var pl = Player.Get(sender);
            if (pl == null || !pl.IsAlive) { response = "Need to be alive to use this!"; return false; }
            if (args.Count < 1)
            {
                response = "Missing argumenst! Correct Usage: spawnentity entity";
                return false;
            }

            response = "Attempting to spawn " + args.First();
            var prot = pl.Rotation.eulerAngles;
            var rot = UnityEngine.Quaternion.Euler(prot.x, 0, 0);
            var ent = SCPRP.Singleton.Entities.SpawnEntity(args.First(), pl.Camera.position + pl.Camera.forward, rot, owner:pl);
            return true;
        }
    }
}
