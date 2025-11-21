using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

namespace SCPRP.Modules
{
    internal class Core : BaseModule
    {
        public override void Load()
        {
            ServerEvents.WaitingForPlayers += WaitingForPlayers;
            ServerEvents.LczDecontaminationStarting += DecontaminationStarting;
        }

        public override void Tick()
        {
           
        }

        public override void Unload()
        {
            ServerEvents.WaitingForPlayers -= WaitingForPlayers;
            ServerEvents.LczDecontaminationStarting -= DecontaminationStarting;
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
    }
}
