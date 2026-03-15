using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using SCPRP.Modules.Players.HUD;
using System.Collections.Generic;
using System.Linq;

namespace SCPRP.Modules.Players.Jobs
{
    public class MayorConfig
    {
        public string[] DefaultLaws { get; set; } = {
           "Do not attack other citizens except in self-defence.",
            "Do not steal or break into people's homes.",
            "Money printers/drugs are illegal.",
        };

        public int MaxLaws { get; set; } = 15;

        public int MaxLawLength { get; set; } = 64;

        public bool ResetLawsOnDeath { get; set; } = true;
    }
    public class Mayor : BaseModule<MayorConfig>
    {
        public static List<string> Laws = new List<string>();

        public static Mayor Singleton;
        public override void Load()
        {
            Laws = Config.DefaultLaws.ToList();
            Singleton = this;

            PlayerEvents.Death += OnDeath;
        }

        public override void Unload()
        {
            PlayerEvents.Death -= OnDeath;
        }

        void OnDeath(PlayerDeathEventArgs e)
        {
            if (Config.ResetLawsOnDeath && e.Player != null && IsMayor(e.Player))
                ResetLaws();
        }

        public static void ResetLaws()
        {
            if (Singleton != null)
                Laws = Singleton.Config.DefaultLaws.ToList();

            Notifications.NotifyAll("The Laws have been reset!");
        }

        public static void AddLaw(string law)
        {
            if (Singleton != null && Laws.Count >= Singleton.Config.MaxLaws)
                return;

            Laws.Add(law);
            Notifications.NotifyAll("The Laws have been changed!");
        }

        public static void RemoveLaw(int i=-1)
        {
            if (i == -1) 
                i = Laws.Count - 1;
            
            if (i < Laws.Count)
                Laws.RemoveAt(i);

            Notifications.NotifyAll("The Laws have been changed!");
        }

        public static bool IsMayor(Player p)
        {
            return p.GetJob() == "mayor";
        }

        public static bool TryAddLaw(Player p, string law)
        {
            if (!IsMayor(p))
            {
                p.Notify("You have to be mayor to add laws!", HUD.Notification.NotifyType.Error);
                return false;
            }

            if (Laws.Count >= Singleton.Config.MaxLaws)
            {
                p.Notify($"Reached the limit of {Singleton.Config.MaxLaws} laws!", HUD.Notification.NotifyType.Error);
                return false;
            }

            law = law.Trim();
            if (law.Length > Singleton.Config.MaxLawLength)
            {
                p.Notify($"Law exceeds limit of {Singleton.Config.MaxLawLength} characters!", HUD.Notification.NotifyType.Error);
                return false;
            }

            AddLaw(law);
            p.Notify("Added the law!", HUD.Notification.NotifyType.Success);
            return true;
        }

        public static bool TryRemoveLaw(Player p, int i=-1)
        {
            if (!IsMayor(p))
            {
                p.Notify("You have to be mayor to remove laws!", HUD.Notification.NotifyType.Error);
                return false;
            }

            if (Laws.Count == 0)
            {
                p.Notify("There are no laws to remove!", HUD.Notification.NotifyType.Error);
                return false;
            }

            if ( i < 0 || i >= Laws.Count)
                i = Laws.Count-1;
            
            if (i < Singleton.Config.DefaultLaws.Length)
            {
                p.Notify("Cannot remove default laws!", HUD.Notification.NotifyType.Error);
                return false;
            }

            var law = Laws[i];
            RemoveLaw(i);

            p.Notify($"Removed \"{law}\"!", HUD.Notification.NotifyType.Success);
            return true;
        }

        public static bool TryResetLaws(Player p)
        {
            if (!IsMayor(p))
            {
                p.Notify("You have to be mayor to reset the laws!", HUD.Notification.NotifyType.Error);
                return false;
            }

            ResetLaws();
            p.Notify($"Reverted the laws to the default laws!", HUD.Notification.NotifyType.Success);
            return true;
        }
    }
}
