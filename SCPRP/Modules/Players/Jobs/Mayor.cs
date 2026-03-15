using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
    public class Mayor : BaseModule<MayorConfig>
    {
        public static List<string> Laws = new List<string>();

        public static Mayor Singleton;
        public override void Load()
        {
            Laws = Config.DefaultLaws.ToList();
            Singleton = this;
        }

        public override void Unload()
        {
            
        }

        public static void ResetLaws()
        {
            if (Singleton != null)
                Laws = Singleton.Config.DefaultLaws.ToList();
        }

        public static void AddLaw(string law)
        {
            if (Singleton != null && Laws.Count >= Singleton.Config.MaxLaws)
                return;
            Laws.Add(law);
        }

        public static void RemoveLaw(int i=-1)
        {
            if (i == -1) 
                i = Laws.Count - 1;
            
            if (i < Laws.Count)
                Laws.RemoveAt(i);
        }

        public static bool TryAddLaw(Player p, string law)
        {
            if (p.GetJob() != "mayor")
            {
                p.Notify("You have to be mayor to add laws!", HUD.Notification.NotifyType.Error);
                return false;
            }

            AddLaw(law);
            p.Notify("Added the law!", HUD.Notification.NotifyType.Success);

            return true;
        }
    }
}
