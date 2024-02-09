using InventoryManagement.API.Validation;

namespace InventoryManagement.API.DTOs
{
    /// <summary>
    /// Represents a data transfer object for creating an inventory item.
    /// </summary>
    public class InventoryItemCreateDto
    {
        /// <summary>
        /// Gets or sets the Stock Keeping Unit (SKU). Acts as a unique identifier for products.
        /// </summary>
        public string? Sku { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product available in the inventory.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the location of the inventory item within a warehouse or storage area.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the inventory item. This is particularly important for perishable goods.
        /// </summary>
        [FutureDate(ErrorMessage = "Expiration date must be today or in the future.")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the batch number for the inventory item. Useful for tracking products from the same production batch.
        /// </summary>
        public string? BatchNumber { get; set; }
    }
}
