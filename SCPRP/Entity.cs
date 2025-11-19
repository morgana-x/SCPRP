using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using SCPRP.Modules.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SCPRP
{
    public abstract class BaseEntity
    {
        public GameObject CoreObject;
        public InteractableToy Interactable;
        public Pickup InteractablePickup;

        public Player Owner = null;
        public abstract void OnTick();
        public abstract void OnInteract(Player player);
        public abstract void OnDamage(Player player, int amount);
        public abstract void OnCreate(UnityEngine.Vector3 position);
        public abstract void OnDestroy();

        public float Health = 0;
        public virtual float MaxHealth => 100;

        public BaseEntity()
        {
        }
        public void Spawn(Vector3 position)
        {
            Health = MaxHealth;
            OnCreate(position);
            SCPRP.Singleton.Entities.AddSpawnedEntity(this);
        }
        public void Destroy()
        {
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
        public void Load()
        {
            var classes = Assembly.GetExecutingAssembly()
                       .GetTypes()
                       .Where(t => t.IsClass && t.Namespace.StartsWith("SCPRP.Entities"))
                       .ToList();

            foreach (var type in classes.Where((x) => { return x.IsSubclassOf(typeof(BaseEntity)); }))
                RegisterEntity(type);


            moduleTickHandle = Timing.RunCoroutine(Tick());
            PlayerEvents.PickingUpItem += OnPicking;
            PlayerEvents.InteractedToy += OnPickingToy;
        }
        public void Unload()
        {
            PlayerEvents.PickingUpItem -= OnPicking;
            PlayerEvents.InteractedToy -= OnPickingToy;
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
        public BaseEntity SpawnEntity(string id, Vector3 pos)
        {
            var ent = CreateEntity(id);
            if (ent != null)
                ent.Spawn(pos);
            if (ent != null) { ent.CoreObject.transform.position = pos; }
            return ent;
        }
    }
}
