using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApiPG.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApiPGContext>
    {
        public ApiPGContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiPGContext>();
            
            // Configuración en tiempo de diseño
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);

            return new ApiPGContext(optionsBuilder.Options);
        }
    }
}
