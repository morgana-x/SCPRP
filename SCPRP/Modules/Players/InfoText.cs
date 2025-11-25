using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Mirror;
using SCPRP.Modules.Players.Jobs;
using UnityEngine;

namespace SCPRP.Modules.Players
{
    internal class InfoText : BaseModule
    {
        public InfoText Singleton;

   
        public override void Load()
        {
            Singleton = this;

            PlayerEvents.Joined += Joined;
            PlayerEvents.Spawned += Spawned;
            PlayerEvents.Left += Left;
        }
        public override void Unload()
        {
            PlayerEvents.Joined -= Joined;
            PlayerEvents.Spawned -= Spawned;
            PlayerEvents.Left -= Left;
        }

        void Joined(PlayerJoinedEventArgs e)
        {
            SyncText(e.Player);
        }
        void Spawned(PlayerSpawnedEventArgs e)
        {
            TextToy toy = TextToy.Create(new UnityEngine.Vector3(0, 0.95f, 0), e.Player.GameObject.transform, false);
            toy.Scale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            toy.TextFormat = "";
            toy.Base.enabled = false;
            toy.Spawn();
         
            if (!Texts.ContainsKey(e.Player))
                Texts.Add(e.Player, toy);
            else
            {
                Texts[e.Player].Destroy();
                Texts[e.Player] = toy;
            }
            SendUpdateTextAll(e.Player);
        }
        void Left(PlayerLeftEventArgs e)
        {
            if (Texts.ContainsKey(e.Player))
            {
                Texts[e.Player].Destroy();
                Texts.Remove(e.Player);
            }
        }

        public Dictionary<Player, TextToy> Texts = new Dictionary<Player, TextToy>();


        TextToy GetTextToy(Player pl)
        {
            return Texts.ContainsKey(pl) ? Texts[pl] : null;
        }
        void UpdateText(Player pl, Player targetToShow)
        {
            if (targetToShow.IsDummy) return;
            if (!targetToShow.IsReady) return;

            var toy = GetTextToy(pl);
            if (toy == null) return;

            if (toy.Base == null) return;
            if (toy.Base.netIdentity == null) return;
            if (toy.IsDestroyed) return;

            string format = "";

            if (pl != targetToShow)
            {
                if (Government.IsWanted(pl))
                    format += "<color=red>!</color><color=blue>WANTED</color><color=red>!</color><br>";
                if (Hitman.GetHit(pl) > 0 && Hitman.IsHitman(targetToShow))
                    format += $"<color=red>Hit ${Hitman.GetHit(pl)}</color><br>";
            }
            targetToShow.SendFakeSyncVar(toy.Base.netIdentity, typeof(AdminToys.TextToy), "Network_textFormat", format);
        }

        void UpdateTextPos(Player pl, Player targetToShow)
        {
            if (targetToShow.IsDummy) return;
            if (!targetToShow.IsReady) return;

            var toy = GetTextToy(pl);
            if (toy == null) return;

            if (toy.Base == null) return;
            if (toy.Base.netIdentity == null) return;
            if (toy.IsDestroyed) return;
            //targetToShow.SendFakeSyncVar(toy.Base, 4, Vector3.one);
            FaceTowardsPlayer(toy, targetToShow);
        }
        public void FaceTowardsPlayer(TextToy toy, Player observer)
        {
            Vector3 direction = observer.Position - toy.Transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(-direction);
            toy.Transform.rotation = rotation;
            observer.SendFakeSyncVar(toy.Base, 2, toy.Transform.localRotation);
        }
        void SendUpdateTextAll(Player pl)
        {
            foreach (var p in Player.GetAll().Where((x) => { return x.Zone == pl.Zone; }))
                UpdateText(pl, p);
        }
        void SendUpdatePosAll(Player pl)
        {
            foreach (var p in Player.GetAll().Where((x) => { return x.Zone == pl.Zone; }))
                UpdateTextPos(pl, p);
        }

        void SyncText(Player pl)
        {
            foreach (var p in Player.GetAll())
            {
                UpdateText(p, pl);
                UpdateTextPos(p, pl);
            }
        }

        DateTime nextTextUpdate = DateTime.Now;
        DateTime nextPosUpdate = DateTime.Now;
        public override void Tick()
        {
            if (nextPosUpdate > DateTime.Now) return;
            nextPosUpdate = DateTime.Now.AddMilliseconds(60);

            if (DateTime.Now > nextTextUpdate)
            {
                nextTextUpdate = DateTime.Now.AddMilliseconds(1500);
                foreach (var player in Texts.Keys.ToList())
                    SendUpdateTextAll(player);
            }
            foreach (var player in Texts.Keys.ToList())
                SendUpdatePosAll(player);
        }
    }
}
