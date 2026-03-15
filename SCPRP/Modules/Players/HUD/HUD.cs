using Hints;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using System;

namespace SCPRP.Modules.Players.HUD
{
    public class HUDConfig
    {
        public bool Enabled { get; set; } = true;
        public string Layout = "<br><br><align=\"center\">{topnotify3}<br>{topnotify2}<br>{topnotify1}</align><br><br><br><br><br><br><br><br><align=right>{notify5}<br>{notify4}<br>{notify3}<br>{notify2}<br>{notify1}<br></align><br>\t<align=left>{job}  <color=#55ff55>${money}</color>\t{wanted}</align>";
    }


    public class HUD : BaseModule<HUDConfig>
    {
        public static HUD Singleton;

        public override void Load()
        {
            Singleton = this;
        }
        public override void Unload()
        {
        }

        DateTime nextHUD = DateTime.Now;

    
        static HintEffect[] HintEffects = new HintEffect[] { new AlphaEffect(255f) };
        public override void Tick()
        {
            if (!Config.Enabled) return;
            if (DateTime.Now < nextHUD) { return; }
            nextHUD = DateTime.Now.AddSeconds(0.5f);
            foreach (var p in Player.GetAll())
            {
                if (!p.IsReady) continue;
                if (p.IsDummy) continue;
                try
                {
                    var hud = Config.Layout.Replace("{job}", Job.GetColouredJobName(p.GetJob())).Replace("{money}", p.GetMoney().ToString()).Replace("\n", "<br>");

                    hud = hud.Replace("{wanted}", Jobs.Government.IsWanted(p) ? "<color=red>Wanted: " + Jobs.Government.GetWantedInfo(p).Reason + "</color>" : "");

                    var notifications = Notifications.GetNotifications(p);
                    for (int i = 0; i < 5; i++)
                        hud = hud.Replace("{notify" + (i + 1).ToString() + "}", notifications.Count > i ? notifications[i].Message : "");

                    var topnotifications = Notifications.GetTopNotifications(p);
                    for (int i = 0; i < 3; i++)
                        hud = hud.Replace("{topnotify" + (i + 1).ToString() + "}", topnotifications.Count > i ? topnotifications[i].Message : "");

                    p.SendHint(hud, HintEffects, 0.7f);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
