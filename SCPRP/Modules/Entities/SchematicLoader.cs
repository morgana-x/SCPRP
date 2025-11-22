using LabApi.Features.Wrappers;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utf8Json;

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


        public class BlockProperties
        {
            public PrimitiveType PrimitiveType { get; set; }
            public string Color { get; set; }
            public AdminToys.PrimitiveFlags PrimitiveFlags { get; set; }
        }

        public class Block
        {
            public string Name { get; set; }
            public int ObjectId { get; set; }
            public int ParentId { get; set; }

            public UnityEngine.Vector3 Position { get; set; }
            public UnityEngine.Vector3 Rotation { get; set; }
            public UnityEngine.Vector3 Scale { get; set; }

            public int BlockType { get; set; }

            public BlockProperties Properties { get; set; }

        }

        public class Schematic
        {
            public int RootObjectId { get; set; }
            public List<Block> Blocks { get; set; }
        }

        public static PrimitiveObjectToy CreateBlock(Block block, Transform parent=null)
        {
            var t = PrimitiveObjectToy.Create(parent);
            t.Type = block.Properties.PrimitiveType;

            t.Position = block.Position;
            t.Scale = block.Scale;
            t.Rotation = Quaternion.Euler(block.Rotation);

            UnityEngine.ColorUtility.TryParseHtmlString(block.Properties.Color, out Color col);
            t.Color = col;

            t.Flags = block.Properties.PrimitiveFlags;

            t.Spawn();

            return t;
        }

        public static PrimitiveObjectToy LoadSchematic(string path)
        {
            if (!System.IO.File.Exists(path))
                return null;

            string text = File.ReadAllText(path);
            Schematic schematic = JsonSerializer.Deserialize<Schematic>(text);

            PrimitiveObjectToy parentGameObject = null;

            Dictionary<int, PrimitiveObjectToy> Loaded = new Dictionary<int, PrimitiveObjectToy>();
            foreach(var block in schematic.Blocks)
            {
                var s = CreateBlock(block, Loaded.ContainsKey(block.ParentId) ? Loaded[block.ParentId].Transform : null);
                Loaded.Add(block.ObjectId, s);
                if (block.ObjectId == schematic.RootObjectId)
                    parentGameObject = s;
            }
            return parentGameObject;
        }
    }
}
