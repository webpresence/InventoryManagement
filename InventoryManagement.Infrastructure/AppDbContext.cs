using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InventoryManagement.Infrastructure.Identity;

namespace InventoryManagement.Infrastructure
{
    /// <summary>
    /// Represents the database context for the Inventory Management system, 
    /// providing access to the application's data through entities.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the database set for Inventory Items.
        /// </summary>
        public DbSet<InventoryItem> InventoryItems { get; set; }
        // Add DbSets for other entities

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types
        /// exposed in <see cref="DbSet{TEntity}" /> properties on your derived context.
        /// The resulting model may be cached and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Model configuration goes here
        }
    }
}
