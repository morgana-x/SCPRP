using LabApi.Features.Extensions;
using MapGeneration;
using PlayerRoles;
using DarkRP.Modules.Players;
using System.Linq;

namespace DarkRP.Modules.Map
{
    public class SpawnDefinition
    {
        public RoleTypeId RoleSpawn { get; set; } = RoleTypeId.None;
        public RoomName RoomSpawn { get; set; } = RoomName.Unnamed;
        public UnityEngine.Vector3 PosSpawn { get; set; } = UnityEngine.Vector3.zero;

        public UnityEngine.Vector3 GetSpawnPosition()
        {
            if (RoomSpawn != RoomName.Unnamed && LabApi.Features.Wrappers.Room.Get(RoomSpawn).Count() > 0)
            {
                return LabApi.Features.Wrappers.Room.Get(RoomSpawn).First().Position + PosSpawn;
            }
            if (RoleSpawn != RoleTypeId.None)
            {
                RoleSpawn.TryGetRandomSpawnPoint(out UnityEngine.Vector3 pos, out float horRot);
                return pos;
            }
            return PosSpawn;
        }

        public SpawnDefinition()
        {

        }

        public SpawnDefinition(RoomName name)
        {
            RoomSpawn = name;
        }

        public SpawnDefinition(RoleTypeId roleSpawn)
        {
            RoleSpawn = roleSpawn;
        }

        public SpawnDefinition(UnityEngine.Vector3 pos)
        {
            PosSpawn = pos;
        }


    }
    public class MapConfig
    {
        public SpawnDefinition Spawnpoint { get; set; } = new SpawnDefinition(RoleTypeId.ClassD);
    }

    public class Map : DarkRPModule<MapConfig>
    {
        public static Map Singleton;
        public override void Load()
        {
            Singleton = this;
        }

        public override void Unload()
        {
            
        }

        public static SpawnDefinition GetSpawn()
        {
            var spawn = Singleton.Config.Spawnpoint;
            if (spawn.GetSpawnPosition() == UnityEngine.Vector3.zero)
                return new SpawnDefinition(RoleTypeId.ClassD);

            return spawn;
        }

        public static SpawnDefinition GetSpawnRole(string role)
        {
            var spawnp = Job.GetJobInfo(role).Spawnpoint;
            if (spawnp.GetSpawnPosition() == UnityEngine.Vector3.zero)
                return GetSpawn();

            return spawnp;
        }
    }
}
