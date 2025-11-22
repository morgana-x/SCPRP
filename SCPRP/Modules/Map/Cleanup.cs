using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPRP.Modules.Map
{
    public class Cleanup : BaseModule
    {

        public Dictionary<Ragdoll, DateTime> Ragdolls = new Dictionary<Ragdoll, DateTime>();
        public override void Load()
        {
            PlayerEvents.SpawnedRagdoll += SpawnedRagdoll;
        }

        public override void Unload()
        {
            PlayerEvents.SpawnedRagdoll -= SpawnedRagdoll;
        }
        public override void Tick()
        {
            foreach(var rag in Ragdolls.Keys.ToList())
            {
                if (DateTime.Now > Ragdolls[rag])
                {
                    rag.Destroy();
                    Ragdolls.Remove(rag);
                }
            }
        }

        void SpawnedRagdoll(PlayerSpawnedRagdollEventArgs e)
        {
            Ragdolls.Add(e.Ragdoll, DateTime.Now.AddSeconds(120));
        }
    }
}
