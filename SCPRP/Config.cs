using SCPRP.Modules.DB;
using SCPRP.Modules.Entity;

namespace SCPRP
{

    public class Config
    {
        public DatabaseConfig DatabaseConfig { get; set; } = new DatabaseConfig();

        public DoorsConfig DoorsConfig { get; set; } = new DoorsConfig();
    }
}
