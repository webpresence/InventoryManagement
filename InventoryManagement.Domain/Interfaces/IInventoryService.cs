using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Domain.Interfaces
{
    /// <summary>
    /// Represents an item within the inventory, tracking stock levels, location, and other important information.
    /// </summary>
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<InventoryItem?> GetByIdAsync(long id);
        Task<InventoryItem> AddAsync(InventoryItem entity);
        Task UpdateAsync(InventoryItem entity);
        Task DeleteAsync(long id);
    }
}
