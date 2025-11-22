using LabApi.Features.Extensions;
using MapGeneration;
using PlayerRoles;
using SCPRP.Entities;
using SCPRP.Modules.DB;
using SCPRP.Modules.Entities;
using SCPRP.Modules.Players;
using System.Linq;
namespace SCPRP
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
        public SpawnDefinition Spawnpoint { get; set; } = new SpawnDefinition(new UnityEngine.Vector3(0.358f, 300.960f, -8.196f));
    }

    public class Config
    {
        public DatabaseConfig DatabaseConfig { get; set; } = new DatabaseConfig();
        
        public MoneyConfig MoneyConfig { get; set; } = new MoneyConfig();

        public MapConfig MapConfig { get; set; } = new MapConfig();
        public DoorsConfig DoorsConfig { get; set; } = new DoorsConfig();

        public Pocket.PocketConfig PocketConfig { get; set; } = new Pocket.PocketConfig();

        public money_printer.MoneyPrinterConfig MoneyPrinterConfig { get; set; } = new money_printer.MoneyPrinterConfig();

        public JobConfig JobConfig { get; set; } = new JobConfig();
        public ShopConfig ShopConfig { get; set; } = new ShopConfig();
    }
}
