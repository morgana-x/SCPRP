using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using SCPRP.Extensions;
using SCPRP.Modules.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCPRP
{
    public abstract class BaseEntity
    {
        public abstract string Name { get; }
        public GameObject CoreObject;
        public InteractableToy Interactable;
        public Pickup InteractablePickup;

        public Player Owner;
        public abstract void OnTick();
        public abstract void OnInteract(Player player);
        public abstract void OnDamage(Player player, int amount);
        public abstract void OnCreate(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation);
        public abstract void OnDestroy();

        public float Health = 0;
        public virtual float MaxHealth => 100;

        public BaseEntity()
        {
        }
        public void Spawn(UnityEngine.Vector3 position)
        {
            Spawn(position, new UnityEngine.Quaternion(0, 0, 0, 0));
        }
        public void Spawn(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation)
        {
            Health = MaxHealth;
            OnCreate(position, rotation);
            SCPRP.Singleton.Entities.AddSpawnedEntity(this);
        }
        public void Destroy()
        {
            OnDestroy();
            SCPRP.Singleton.Entities.RemoveSpawnedEntity(this);
            if (CoreObject != null)
                NetworkServer.Destroy(CoreObject);
        }

    }

    public class Entity
    {

        public Dictionary<string, Type> EntityMap = new Dictionary<string, Type>();
        private volatile List<BaseEntity> entities = new List<BaseEntity>();

        public List<BaseEntity> Entities {  get { return entities.ToList(); } }

        public CoroutineHandle moduleTickHandle;

        public static Entity Singleton;
        public void Load()
        {
            Singleton = this;
            var classes = Assembly.GetExecutingAssembly()
                       .GetTypes()
                       .Where(t => t.IsClass && t.Namespace.StartsWith("SCPRP.Entities"))
                       .ToList();

            foreach (var type in classes.Where((x) => { return x.IsSubclassOf(typeof(BaseEntity)); }))
                RegisterEntity(type);


            moduleTickHandle = Timing.RunCoroutine(Tick());
            PlayerEvents.PickingUpItem += OnPicking;
            PlayerEvents.InteractedToy += OnPickingToy;
            PlayerEvents.PlacedBulletHole += ShotWeapon;
        }
        public void Unload()
        {
            PlayerEvents.PickingUpItem -= OnPicking;
            PlayerEvents.InteractedToy -= OnPickingToy;
            PlayerEvents.PlacedBulletHole -= ShotWeapon;
            Timing.KillCoroutines(moduleTickHandle);
            entities.Clear();
            EntityMap.Clear();
        }
        private IEnumerator<float> Tick()
        {
            while (true)
            {
                foreach (var ent in Entities)
                {
                    try
                    {
                        ent.OnTick();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                yield return MEC.Timing.WaitForSeconds(0.1f);
            }
        }
        public static BaseEntity GetEntity(GameObject obj)
        {
            var result = Singleton.Entities.Where((x) => { return (x.CoreObject!=null && x.CoreObject == obj) ||  (x.Interactable != null && x.Interactable.GameObject == obj) || (x.InteractablePickup != null && x.InteractablePickup.Base.gameObject == obj); }).ToList();
            return result.Count > 0 ? result.First() : null;
        }
        private void ShotWeapon(PlayerPlacedBulletHoleEventArgs e)
        {
            var result = Physics.Raycast(e.RaycastStart, (e.HitPosition - e.RaycastStart).normalized, out RaycastHit info, 9999f);
            if (!result) return;
            if (info.collider == null || info.collider.gameObject == null) return;
            var ent = GetEntity(info.collider.gameObject);
            if (ent == null) return;
            ent.OnDamage(e.Player, 20);
        }
        private void OnPicking(PlayerPickingUpItemEventArgs e)
        {
            foreach (var ent in Entities)
            {
                if (e.Pickup != ent.InteractablePickup) continue;
                e.IsAllowed = false;
                e.Pickup.Spawn();
                try
                {
                    ent.OnInteract(e.Player);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return;
            }
        }
        private void OnPickingToy(PlayerInteractedToyEventArgs e)
        {
            foreach (var ent in Entities)
            {
                if (e.Interactable != ent.Interactable) continue;
                try
                {
                    ent.OnInteract(e.Player);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return;
            }
        }
        public void RegisterEntity(Type type)
        {
            EntityMap.Add(type.Name, type);
            LabApi.Features.Console.Logger.Info($"Registered entity {type.Name}!");
        }

        internal void AddSpawnedEntity(BaseEntity entity)
        {
            if (!Entities.Contains(entity))
                entities.Add(entity);
        }
        internal void RemoveSpawnedEntity(BaseEntity entity)
        {
            if (Entities.Contains(entity))
                entities.Remove(entity);
        }
        public BaseEntity CreateEntity(string id)
        {
            if (!EntityMap.ContainsKey(id)) { return null; }
            return (BaseEntity)Activator.CreateInstance(EntityMap[id]);
        }
        public BaseEntity SpawnEntity(string id, UnityEngine.Vector3 pos, Player owner = null)
        {
            return SpawnEntity(id, pos, new UnityEngine.Quaternion(0,0,0,0), owner);
        }
        public BaseEntity SpawnEntity(string id, UnityEngine.Vector3 pos, UnityEngine.Quaternion rotation, Player owner=null)
        {
            var ent = CreateEntity(id);
            if (ent == null) return null;
            ent.Owner = owner;
            ent.Spawn(pos, rotation);
            return ent;
        }

        public List<BaseEntity> GetEntities(Player p, string id="")
        {
            return Entities.Where((x) =>
            {
                return (x.Owner != null && x.Owner == p) && (id == "" || (x.GetType().Name == id));
            }
            ).ToList();
        }
    }
}
