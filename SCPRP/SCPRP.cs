using LabApi.Features;
using LabApi.Loader.Features.Plugins;
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

        public ModuleManager Modules;

        public static SCPRP Singleton;

   
        public override void Enable()
        {
            Singleton = this;
            Modules = new ModuleManager();
            Modules.Load();
        }

        public override void Disable()
        {
            Modules.Unload();
        }


    }

}
