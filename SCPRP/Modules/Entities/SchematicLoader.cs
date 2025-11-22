using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPRP.Modules.Entities
{
    public class SchematicLoader : BaseModule
    {
        public override void Load()
        {
            
        }

        public override void Tick()
        {
            
        }

        public override void Unload()
        {
            
        }

        public static GameObject LoadSchematic(string path)
        {
            return PrimitiveObjectToy.Create().GameObject;
        }
    }
}
