using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Domain.Entities
{
    /// <summary>
    /// Represents an item within the inventory, tracking stock levels, location, and other important information.
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// Gets or sets the primary key for the inventory item.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Stock Keeping Unit (SKU). Acts as a unique identifier for products.
        /// </summary>
        /// <remarks>
        /// While marked as nullable, ensure SKU is provided for better inventory management.
        /// </remarks>
        [StringLength(50)]
        public string? Sku { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product available in the inventory.
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the location of the inventory item within a warehouse or storage area.
        /// </summary>
        /// <remarks>
        /// Can be used to identify specific shelves, bins, or areas for efficient inventory management.
        /// </remarks>
        [StringLength(100)]
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the inventory item. This is particularly important for perishable goods.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the batch number for the inventory item. Useful for tracking products from the same production batch.
        /// </summary>
        [StringLength(50)]
        public string? BatchNumber { get; set; }
    }
}
