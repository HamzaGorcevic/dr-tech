using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace drTech_backend.Infrastructure
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var conn = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
            optionsBuilder.UseNpgsql(conn);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}


