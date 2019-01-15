using Microsoft.EntityFrameworkCore;

namespace StartupApi.Model
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppDataEntity> AppDatas { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
