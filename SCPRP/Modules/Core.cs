using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SCPRP.Modules.Players;
using UnityEngine;

namespace SCPRP.Modules
{
    internal class Core : BaseModule
    {
        public override void Load()
        {
            ServerEvents.WaitingForPlayers += WaitingForPlayers;
            ServerEvents.LczDecontaminationStarting += DecontaminationStarting;
            PlayerEvents.Escaping += Escaping;

            PlayerEvents.Joined += Joined;
        }

        public override void Tick()
        {
           
        }

        public override void Unload()
        {
            ServerEvents.WaitingForPlayers -= WaitingForPlayers;
            ServerEvents.LczDecontaminationStarting -= DecontaminationStarting;
            PlayerEvents.Escaping -= Escaping;


            PlayerEvents.Joined -= Joined;
        }

        void Joined(PlayerJoinedEventArgs e)
        {
            e.Player.SendConsoleMessage($"<color=yellow>Welcome to SCPRP!</color> Type <color=#16def3>.rphelp</color> to get started!</size>");
            e.Player.SendConsoleMessage("Remember to bind the <color=green>Buy Door</color> key in <color=yellow>Server Specific</color> Settings!");
            HUD.ShowHint(e.Player, "<size=50><color=yellow>OPEN CONSOLE (</color><color=#16def3>~</color><color=yellow>) TO GET STARTED!!!</color></size>", 10f);
        }
        void DecontaminationStarting(LczDecontaminationStartingEventArgs e)
        {
            e.IsAllowed = false;
        }
        void WaitingForPlayers()
        {
            Round.IsLocked = true;
            Server.FriendlyFire = true;
            Round.Start();
            
        }

        void Escaping(PlayerEscapingEventArgs e)
        {
            e.IsAllowed = false;
        }
    }
}
