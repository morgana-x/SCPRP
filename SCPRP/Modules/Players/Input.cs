using UserSettings.ServerSpecific;

namespace SCPRP.Modules.Players
{
    public class Input : BaseModule
    {
        public enum InputIds
        {
            BuyDoor = 875,
        }

        public override void Load()
        {
            ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[]
            {
                new SSGroupHeader("Actions"),
                new SSKeybindSetting((int)InputIds.BuyDoor, "Buy Door", UnityEngine.KeyCode.F2, allowSpectatorTrigger:false)
            };
            ServerSpecificSettingsSync.SendOnJoinFilter = (x) => { return true; };
        }

        public override void Unload()
        {
            
        }

        public override void Tick()
        {
        
        }
    
    }
}
