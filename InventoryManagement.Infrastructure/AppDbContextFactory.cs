using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.Infrastructure
{
    /// <summary>
    /// Factory for creating instances of <see cref="AppDbContext"/> with design-time support for EF Core tooling.
    /// </summary>
    internal class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AppDbContext"/> configured with options 
        /// obtained from the application's configuration settings.
        /// </summary>
        /// <param name="args">Arguments provided through the command line. Not used in this implementation.</param>
        /// <returns>A new instance of <see cref="AppDbContext"/>.</returns>
        /// <remarks>
        /// This method is primarily intended for use by EF Core command-line tools during design-time operations 
        /// such as migrations. It reads the database connection string from the application's configuration 
        /// file (appsettings.json) located in the parent directory of the Infrastructure project.
        /// </remarks>
        public AppDbContext CreateDbContext(string[] args)
        {
            // Assuming your appsettings.json is in the parent directory of the Infrastructure project,
            // adjust the path as necessary.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../InventoryManagement.Api"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Ensure you use the correct method for your database provider, e.g., UseSqlServer for SQL Server.
            builder.UseSqlServer(connectionString);

            return new AppDbContext(builder.Options);
        }
    }
}
