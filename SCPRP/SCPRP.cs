using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using SCPRP.Modules.Items;
using System;

namespace SCPRP
{
    public class SCPRP : Plugin<Config>
    {
        public override string Name => "SCPRP";

        public override string Description => "SCP RP Base Plugin";

        public override string Author => "morgana";

        public override Version Version => new Version(1, 0);

        public override Version RequiredApiVersion => new Version(LabApiProperties.CompiledVersion);

        public Module Modules;
        public Entity Entities;

        public static SCPRP Singleton;

   
        public override void Enable()
        {
            Singleton = this;

            Modules = new Module();
            Modules.Load();

            Entities = new Entity();
            Entities.Load();

        }

        public override void Disable()
        {
            Modules.Unload();
            Entities.Unload();
        }


    }

}
