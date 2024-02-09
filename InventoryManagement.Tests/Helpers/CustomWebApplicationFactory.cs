using InventoryManagement.Infrastructure.Identity;
using InventoryManagement.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using InventoryManagement.Domain.Entities;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the application's DbContext configuration.
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Reconfigure the authentication to use in-memory store for users and roles
            var identityBuilder = new IdentityBuilder(typeof(ApplicationUser), typeof(IdentityRole), services);
            identityBuilder.AddEntityFrameworkStores<AppDbContext>();
            identityBuilder.AddDefaultTokenProviders();

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (AppDbContext).
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                if (!db.InventoryItems.Any())
                {
                    db.InventoryItems.AddRange(
                        new InventoryItem { Id = 1, BatchNumber = "abc", ExpirationDate = DateTime.Now.AddDays(1), Location = "B4", Quantity=100, Sku ="x01" },
                        new InventoryItem { Id = 2, BatchNumber = "abc1", ExpirationDate = DateTime.Now.AddDays(1), Location = "B5", Quantity = 100, Sku = "x02" }
                    );
                    db.SaveChanges();
                }
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data if necessary.
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
                }
            }
        });
    }
}
