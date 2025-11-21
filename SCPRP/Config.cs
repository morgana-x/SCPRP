using SCPRP.Modules.DB;
using SCPRP.Modules.Entities;
using SCPRP.Modules.Players;

namespace SCPRP
{

    public class Config
    {
        public DatabaseConfig DatabaseConfig { get; set; } = new DatabaseConfig();
        
        public MoneyConfig MoneyConfig { get; set; } = new MoneyConfig();
        public DoorsConfig DoorsConfig { get; set; } = new DoorsConfig();

        public JobConfig JobConfig { get; set; } = new JobConfig();
        public ShopConfig ShopConfig { get; set; } = new ShopConfig();
    }
}
