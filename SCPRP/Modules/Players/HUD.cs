using Hints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SCPRP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPRP.Modules.Players
{
    public class HUDConfig
    {
        public bool Enabled { get; set; } = true;
        public string Layout = "<br><br><align=\"center\">{topnotify3}<br>{topnotify2}<br>{topnotify1}</align><br><br><br><br><br><br><br><br><align=right>{notify5}<br>{notify4}<br>{notify3}<br>{notify2}<br>{notify1}<br></align><br>\t<align=left>{job} <color=#55ff55>${money}</color>\t{wanted}</align>";
    }

    public class Notification
    {
        public DateTime Expire;
        public string Message;

        public Notification(string msg, float seconds)
        {
            Message = msg;
            Expire = DateTime.Now.AddSeconds(seconds);
        }
    }

    public class HUD : BaseModule<HUDConfig>
    {
        public static HUD Singleton;

        public Dictionary<Player, List<Notification>> Notifications = new Dictionary<Player, List<Notification>>();
        public Dictionary<Player, List<Notification>> TopNotifications = new Dictionary<Player, List<Notification>>();
        public override void Load()
        {
            Singleton = this;
            PlayerEvents.Left += Left;
        }
        public override void Unload()
        {
            PlayerEvents.Left -= Left;
        }

        DateTime nextHUD = DateTime.Now;

        void Left(PlayerLeftEventArgs e)
        {
            if (Notifications.ContainsKey(e.Player))
                Notifications.Remove(e.Player);
            if (TopNotifications.ContainsKey(e.Player))
                TopNotifications.Remove(e.Player);
        }
        public List<Notification> GetNotifications(Player player)
        {
            return Notifications.ContainsKey(player) ? Notifications[player] : new List<Notification>();
        }
        public List<Notification> GetTopNotifications(Player player)
        {
            return TopNotifications.ContainsKey(player) ? TopNotifications[player] : new List<Notification>();
        }
        void CheckExpiredNotifications(Player player)
        {
            if (Notifications.ContainsKey(player) && Notifications[player].Count > 0)
            {
                var first = Notifications[player].Last();
                if (DateTime.Now > first.Expire)
                    Notifications[player].Remove(first);

            }
            if (TopNotifications.ContainsKey(player) && TopNotifications[player].Count > 0)
            {
                var first = TopNotifications[player].Last();
                if (DateTime.Now > first.Expire)
                    TopNotifications[player].Remove(first);

            }
        }
        static HintEffect[] HintEffects = new HintEffect[] { new AlphaEffect(255f) };
        public override void Tick()
        {
            if (!Config.Enabled) return;
            if (DateTime.Now < nextHUD) { return;  }
            nextHUD = DateTime.Now.AddSeconds(0.5f);
            foreach (var p in LabApi.Features.Wrappers.Player.GetAll())
            {
                if (!p.IsReady) continue;
                if (p.IsDummy) continue;
                try
                {
                    var hud = Config.Layout.Replace("{job}", Job.GetColouredJobName(p.GetJob())).Replace("{money}", p.GetMoney().ToString()).Replace("\n", "<br>");

                    hud = hud.Replace("{wanted}", Modules.Players.Jobs.Government.IsWanted(p) ? "<color=red>Wanted: " + Jobs.Government.GetWantedInfo(p).Reason +"</color>" : "");
                    CheckExpiredNotifications(p);

                    var notifications = GetNotifications(p);
                    for (int i=0; i < 5;i++)
                        hud = hud.Replace("{notify"+(i+1).ToString()+"}", notifications.Count > i ? notifications[i].Message : "");

                    var topnotifications = GetTopNotifications(p);
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

        public static void Notify(LabApi.Features.Wrappers.Player pl, string text, float duration = 8f)
        {
            if (Singleton == null) return;
            if (!Singleton.Notifications.ContainsKey(pl))
                Singleton.Notifications.Add(pl, new List<Notification>());

            Singleton.Notifications[pl].Insert(0, new Notification(text, duration));
        }

        public static void NotifyTop(Player pl, string text, float duration=8f)
        {
            if (Singleton == null) return;
            if (!Singleton.TopNotifications.ContainsKey(pl))
                Singleton.TopNotifications.Add(pl, new List<Notification>());

            Singleton.TopNotifications[pl].Insert(0,new Notification(text, duration));
        }

        public static void NotifyAll(string text, float duration=8f, bool top=false)
        {
            foreach(var p in Player.GetAll())
            {
                if (top)
                    NotifyTop(p, text, duration);
                else
                    Notify(p, text, duration);
            }
        }

    }
}
