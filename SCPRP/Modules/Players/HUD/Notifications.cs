using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using static SCPRP.Modules.Players.HUD.Notification;

namespace SCPRP.Modules.Players.HUD
{
    public class Notification
    {
        public enum NotifyType
        {
            Info,
            Warn,
            Error,
            Success,
            Hint
        }

        public DateTime Expire;
        public string Message;
        public NotifyType MsgType;

        public Notification(string msg, float seconds=8f, NotifyType type=NotifyType.Info)
        {
            Message = msg;
            Expire = DateTime.Now.AddSeconds(seconds);
            MsgType = type;
        }
    }

    public class  NotificationsConfig
    {
        public Dictionary<NotifyType, string> Colours { get; set; } = new Dictionary<NotifyType, string>()
        {
            [NotifyType.Info] = "#48a1f1",
            [NotifyType.Warn] = "#f1d948",
            [NotifyType.Error] = "#f14852",
            [NotifyType.Success] = "#96f148",
            [NotifyType.Hint] = "#f1b348"
        };
    }

    public class Notifications : BaseModule<NotificationsConfig>
    {
        public Dictionary<Player, List<Notification>> SideNotifications = new Dictionary<Player, List<Notification>>();
        public Dictionary<Player, List<Notification>> TopNotifications = new Dictionary<Player, List<Notification>>();

        public static Notifications Singleton;
        public override void Load()
        {
            PlayerEvents.Left += Left;
        }

        public override void Unload()
        {
            PlayerEvents.Left -= Left;
        }

        void Left(PlayerLeftEventArgs e)
        {
            SideNotifications.Remove(e.Player);
            TopNotifications.Remove(e.Player);
        }

        public static List<Notification> GetNotifications(Player player)
        {
            if (Singleton == null) return new List<Notification> { };

            CheckExpiredNotifications(player);
            return Singleton.SideNotifications.ContainsKey(player) ? Singleton.SideNotifications[player] : new List<Notification>();
        }
        public static List<Notification> GetTopNotifications(Player player)
        {
            if (Singleton == null) return new List<Notification> { };

            CheckExpiredNotifications(player);
            return Singleton.TopNotifications.ContainsKey(player) ? Singleton.TopNotifications[player] : new List<Notification>();
        }

        public static void Notify(Player pl, string text, float duration = 8f, NotifyType type=NotifyType.Info)
        {
            if (Singleton == null) return;
            if (!Singleton.SideNotifications.ContainsKey(pl))
                Singleton.SideNotifications.Add(pl, new List<Notification>());

            Singleton.SideNotifications[pl].Insert(0, new Notification(text, duration));

            pl.SendConsoleMessage(text, Singleton.Config.Colours[type]);
        }

        public static void NotifyTop(Player pl, string text, float duration = 8f, NotifyType type = NotifyType.Info)
        {
            if (Singleton == null) return;
            if (!Singleton.TopNotifications.ContainsKey(pl))
                Singleton.TopNotifications.Add(pl, new List<Notification>());

            Singleton.TopNotifications[pl].Insert(0, new Notification(text, duration));

            pl.SendConsoleMessage(text, Singleton.Config.Colours[type]);
        }

        public static void NotifyAll(string text, float duration = 8f, bool top = false, NotifyType type = NotifyType.Info)
        {
            foreach (var p in Player.GetAll())
            {
                if (top)
                    NotifyTop(p, text, duration, type);
                else
                    Notify(p, text, duration, type);
            }
        }

        private static void CheckExpiredNotifications(Player player)
        {
            if (Singleton == null) return;
            if (Singleton.SideNotifications.ContainsKey(player) && Singleton.SideNotifications[player].Count > 0)
            {
                var first = Singleton.SideNotifications[player].Last();
                if (DateTime.Now > first.Expire)
                    Singleton.SideNotifications[player].Remove(first);

            }
            if (Singleton.TopNotifications.ContainsKey(player) && Singleton.TopNotifications[player].Count > 0)
            {
                var first = Singleton.TopNotifications[player].Last();
                if (DateTime.Now > first.Expire)
                    Singleton.TopNotifications[player].Remove(first);

            }
        }
    }
}
