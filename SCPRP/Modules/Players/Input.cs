using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using System.Collections.Generic;
using System.Linq;
using UserSettings.ServerSpecific;

namespace SCPRP.Modules.Players
{
    public class Input : BaseModule
    {
        public enum InputIds
        {
            BuyDoor = 875,
        }

        void CreateSettings() // Perhaps some other plugins *cough* mer *cough* should consider this approach >:(
        {
            List<ServerSpecificSettingBase> Items = new List<ServerSpecificSettingBase>();

            if (ServerSpecificSettingsSync.DefinedSettings != null)
                Items = ServerSpecificSettingsSync.DefinedSettings.ToList();

            Items.Add(new SSGroupHeader("Actions"));
            Items.Add(new SSKeybindSetting((int)InputIds.BuyDoor, "Buy Door", UnityEngine.KeyCode.F2, allowSpectatorTrigger: false));

            ServerSpecificSettingsSync.DefinedSettings = Items.ToArray();

            ServerSpecificSettingsSync.SendOnJoinFilter = (x) => { return true; };
            ServerSpecificSettingsSync.SendToAll();
        }

        public override void Load()
        {
            ServerEvents.WaitingForPlayers += CreateSettings;
        }

        public override void Unload()
        {
            ServerEvents.WaitingForPlayers -= CreateSettings;
        }

        public override void Tick()
        {
        
        }
    
    }
}
