using CommandSystem.Commands.RemoteAdmin.Decontamination;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using SCPRP.Modules.Players;
using System;
using System.IO;
using UnityEngine;

namespace SCPRP.Modules
{
    internal class Core : BaseModule
    {
        public override void Load()
        {
            ServerEvents.WaitingForPlayers += WaitingForPlayers;

            PlayerEvents.InteractingWarheadLever += InteractingWarhead;

            PlayerEvents.Escaping += Escaping;

            PlayerEvents.Joined += Joined;


         //   PlayerEvents.Death += Death;
     
        }

        public override void Tick()
        {
           
        }

        public override void Unload()
        {
            ServerEvents.WaitingForPlayers -= WaitingForPlayers;
            PlayerEvents.Escaping -= Escaping;

            PlayerEvents.InteractingWarheadLever -= InteractingWarhead;

            PlayerEvents.Joined -= Joined;

           // PlayerEvents.Death -= Death;
        }

        void Joined(PlayerJoinedEventArgs e)
        {
            e.Player.SendConsoleMessage($"<color=yellow>Welcome to SCPRP!</color> Type <color=#16def3>.rphelp</color> to get started!</size>");
            e.Player.SendConsoleMessage("Remember to bind the <color=green>Buy Door</color> key in <color=yellow>Server Specific</color> Settings!");
            HUD.NotifyTop(e.Player, "<color=yellow>OPEN CONSOLE (</color><color=#16def3>~</color><color=yellow>) TO GET STARTED!!!</color>", 12f);
        }

        void WaitingForPlayers()
        {
            Round.IsLocked = true;
            Server.FriendlyFire = true;
            DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
            Round.Start();
            
        }

        void Escaping(PlayerEscapingEventArgs e)
        {
            e.IsAllowed = false;
        }

        void InteractingWarhead(PlayerInteractingWarheadLeverEventArgs e)
        {
            e.IsAllowed = false;
            e.Enabled = false;
        }

        void Death(PlayerDeathEventArgs e)
        {
            e.Player.Health = e.Player.MaxHealth;
        }

    }
}
