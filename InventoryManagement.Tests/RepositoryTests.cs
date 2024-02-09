using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Tests
{
    public class RepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public RepositoryTests()
        {
            // Arrange: Ensure each test method runs with a fresh in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllItems_WithCorrectProperties()
        {
            // Arrange: Seed data with initialized properties
            var expectedItems = new List<InventoryItem>();
            using (var setupContext = new AppDbContext(_dbContextOptions))
            {
                expectedItems = new List<InventoryItem>
                {
                    new InventoryItem
                    {
                        Sku = "SKU1001",
                        Quantity = 10,
                        Location = "Aisle 1",
                        ExpirationDate = DateTime.UtcNow.AddDays(100),
                        BatchNumber = "BATCH001"
                    },
                    new InventoryItem
                    {
                        Sku = "SKU1002",
                        Quantity = 20,
                        Location = "Aisle 2",
                        ExpirationDate = DateTime.UtcNow.AddDays(200),
                        BatchNumber = "BATCH002"
                    }
                };

                setupContext.InventoryItems.AddRange(expectedItems);
                await setupContext.SaveChangesAsync();
            }

            List<InventoryItem> items;

            // Act: Use a separate context instance to retrieve the items
            using (var assertContext = new AppDbContext(_dbContextOptions))
            {
                items = await assertContext.InventoryItems.ToListAsync();
            }

            // Assert: Verify the correct items and properties were returned
            Assert.Equal(2, items.Count); // Verify that exactly 2 items were returned

            foreach (var item in items)
            {
                var expectedItem = expectedItems.FirstOrDefault(e => e.Sku == item.Sku);
                Assert.NotNull(expectedItem); // Ensure an expected item matches the returned item

                Assert.Equal(expectedItem.Sku, item.Sku);
                Assert.Equal(expectedItem.Quantity, item.Quantity);
                Assert.Equal(expectedItem.Location, item.Location);
                Assert.Equal(expectedItem.ExpirationDate?.Date, item.ExpirationDate?.Date);
                Assert.Equal(expectedItem.BatchNumber, item.BatchNumber);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsItem_WhenItemExists()
        {
            // Arrange: Seed the database with an item
            long itemId;
            var expectedItem = new InventoryItem
            {
                Sku = "SKU12345",
                Quantity = 100,
                Location = "Warehouse 1",
                ExpirationDate = DateTime.UtcNow.AddDays(10),
                BatchNumber = "Batch123"
            };

            using (var setupContext = new AppDbContext(_dbContextOptions))
            {
                setupContext.InventoryItems.Add(expectedItem);
                await setupContext.SaveChangesAsync();
                itemId = expectedItem.Id; // Assuming Id is auto-generated
            }

            InventoryItem retrievedItem;

            // Act: Retrieve the item by ID using a separate context instance
            using (var assertContext = new AppDbContext(_dbContextOptions))
            {
                retrievedItem = await assertContext.InventoryItems.FindAsync(itemId);
            }

            // Assert: Verify the retrieved item matches the expected item
            Assert.NotNull(retrievedItem);
            Assert.Equal(expectedItem.Id, retrievedItem.Id);
            Assert.Equal(expectedItem.Sku, retrievedItem.Sku);
            Assert.Equal(expectedItem.Quantity, retrievedItem.Quantity);
            Assert.Equal(expectedItem.Location, retrievedItem.Location);
            Assert.Equal(expectedItem.ExpirationDate?.Date, retrievedItem.ExpirationDate?.Date);
            Assert.Equal(expectedItem.BatchNumber, retrievedItem.BatchNumber);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesItemCorrectly()
        {
            // Arrange: Seed the database with an item to update
            InventoryItem originalItem;
            using (var setupContext = new AppDbContext(_dbContextOptions))
            {
                var item = new InventoryItem
                {
                    Sku = "SKU123",
                    Quantity = 50,
                    Location = "Warehouse A",
                    ExpirationDate = DateTime.UtcNow.AddDays(30),
                    BatchNumber = "Batch1234"
                };
                setupContext.InventoryItems.Add(item);
                await setupContext.SaveChangesAsync();
                originalItem = item;
            }

            // Act: Retrieve and update the item in the database
            using (var updateContext = new AppDbContext(_dbContextOptions))
            {
                var itemToUpdate = await updateContext.InventoryItems.FindAsync(originalItem.Id);
                itemToUpdate.Quantity = 10; // Example change
                updateContext.Entry(itemToUpdate).State = EntityState.Modified;
                await updateContext.SaveChangesAsync();
            }

            // Assert: Verify that the item was updated correctly
            using (var assertContext = new AppDbContext(_dbContextOptions))
            {
                var updatedItem = await assertContext.InventoryItems.FindAsync(originalItem.Id);
                Assert.NotNull(updatedItem);
                Assert.Equal(originalItem.Id, updatedItem.Id);
                Assert.Equal("SKU123", updatedItem.Sku);
                Assert.Equal(10, updatedItem.Quantity);
                Assert.Equal("Warehouse A", updatedItem.Location);
            }
        }
    }
}
