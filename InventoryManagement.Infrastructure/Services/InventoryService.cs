using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Services
{
    /// <summary>
    /// Provides services for managing inventory items.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all inventory items from the database asynchronously.
        /// </summary>
        /// <returns>A list of inventory items.</returns>
        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await _context.InventoryItems.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific inventory item by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the inventory item.</param>
        /// <returns>The inventory item if found; otherwise, null.</returns>
        public async Task<InventoryItem?> GetByIdAsync(long id)
        {
            return await _context.InventoryItems.FindAsync(id);
        }

        /// <summary>
        /// Adds a new inventory item to the database asynchronously.
        /// </summary>
        /// <param name="entity">The inventory item to add.</param>
        /// <returns>The added inventory item.</returns>
        public async Task<InventoryItem> AddAsync(InventoryItem entity)
        {
            _context.InventoryItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates an existing inventory item in the database asynchronously.
        /// </summary>
        /// <param name="entity">The inventory item to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync(InventoryItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an inventory item from the database asynchronously based on its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the inventory item to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(long id)
        {
            var entity = await _context.InventoryItems.FindAsync(id);
            if (entity != null)
            {
                _context.InventoryItems.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
