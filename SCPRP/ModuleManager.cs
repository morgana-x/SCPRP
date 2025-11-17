using LabApi.Features.Console;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SCPRP
{
    public abstract class BaseModule
    {
        public abstract void Load();
        public abstract void Unload();
        public abstract void Tick();
    }
    public class ModuleManager
    {

        public Dictionary<Type, object> LoadedModules = new Dictionary<Type, object>();

        public CoroutineHandle moduleTickHandle;
        public void Load()
        {
            var classes = Assembly.GetExecutingAssembly()
                       .GetTypes()
                       .Where(t => t.IsClass && t.Namespace.StartsWith("SCPRP.Modules"))
                       .ToList();

           foreach (var type in classes.Where((x) => { return x.IsSubclassOf(typeof(BaseModule)); }))
           {
                AddModule(type);
           }

            moduleTickHandle = Timing.RunCoroutine(Tick());
        }

        public void AddModule(Type type)
        {
            var module = Activator.CreateInstance(type);
            ((BaseModule)module).Load();
            LoadedModules.Add(type, module);
            Logger.Info($"Loaded module {type.Name}");
        }

        public object GetModule(Type type)
        {
            if (LoadedModules.ContainsKey(type)) { return LoadedModules[type]; }
            return null;
        }

        public object GetModule<Type>() {  return GetModule(typeof(Type)); }

        public void Unload()
        {
            foreach (var pair in LoadedModules)
            {
                ((BaseModule)pair.Value).Unload();
            }
            LoadedModules.Clear();
        }

        private IEnumerator<float> Tick()
        {
            while (true)
            {
                foreach (var pair in LoadedModules)
                {
                    try
                    {
                        ((BaseModule)pair.Value).Tick();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                yield return MEC.Timing.WaitForSeconds(0.1f);
            }
        }
    }
}
