using Microsoft.EntityFrameworkCore;

namespace Deviceflow_ConsoleApp.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext > options) : base(options) { }

        public DbSet<DeviceFlowClientEntity> DeviceFlowClients { get; set; }

    }
}
