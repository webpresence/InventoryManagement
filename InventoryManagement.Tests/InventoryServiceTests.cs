using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

public class InventoryServiceTests
{

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        var _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase1")
            .Options;
        // Arrange
        using (var context = new AppDbContext(_options))
        {
            var expectedItems = new List<InventoryItem>
        {
            new InventoryItem
            {
                Sku = "SKU12345",
                Quantity = 10,
                Location = "Aisle 1, Shelf 1",
                ExpirationDate = new DateTime(2023, 12, 31),
                BatchNumber = "BATCH001"
            },
            new InventoryItem
            {
                Sku = "SKU54321",
                Quantity = 20,
                Location = "Aisle 2, Shelf 3",
                ExpirationDate = new DateTime(2024, 06, 30),
                BatchNumber = "BATCH002"
            }
        };
            context.InventoryItems.AddRange(expectedItems);
            context.SaveChanges();
        }

        // Act
        List<InventoryItem> result;
        using (var context = new AppDbContext(_options))
        {
            var service = new InventoryService(context);
            result = (await service.GetAllAsync()).ToList();
        }

        // Assert
        Assert.Equal(2, result.Count);

        var item1 = result.FirstOrDefault(i => i.Sku == "SKU12345");
        Assert.NotNull(item1);
        Assert.Equal(10, item1?.Quantity);
        Assert.Equal("Aisle 1, Shelf 1", item1?.Location);
        Assert.Equal(new DateTime(2023, 12, 31), item1?.ExpirationDate);
        Assert.Equal("BATCH001", item1?.BatchNumber);


        var item2 = result.FirstOrDefault(i => i.Sku == "SKU54321");
        Assert.NotNull(item2);
        Assert.Equal(20, item2?.Quantity);
        Assert.Equal("Aisle 2, Shelf 3", item2?.Location);
        Assert.Equal(new DateTime(2024, 06, 30), item2?.ExpirationDate);
        Assert.Equal("BATCH002", item2?.BatchNumber);
    }


    [Fact]
    public async Task GetByIdAsync_ReturnsItem_WhenItemExists()
    {
        var _options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDatabase")
          .Options;

        // Arrange
        long itemId;
        var expectedItem = new InventoryItem
        {
            Sku = "SKU123",
            Quantity = 10,
            Location = "Warehouse A",
            ExpirationDate = DateTime.Now.AddDays(30),
            BatchNumber = "Batch123"
            // Add any other properties as needed
        };

        using (var context = new AppDbContext(_options))
        {
            context.InventoryItems.Add(expectedItem);
            context.SaveChanges();
            itemId = expectedItem.Id;
        }

        // Act
        InventoryItem result;
        using (var context = new AppDbContext(_options))
        {
            var service = new InventoryService(context);
            result = await service.GetByIdAsync(itemId);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItem, result, new InventoryItemEqualityComparer());
    }


    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenItemDoesNotExist()
    {
        var _options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDatabase")
          .Options;

        // Arrange
        long nonExistentItemId = 999;

        // Act
        InventoryItem result;
        using (var context = new AppDbContext(_options))
        {
            var service = new InventoryService(context);
            result = await service.GetByIdAsync(nonExistentItemId);
        }

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsItemCorrectly()
    {
        var _options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDatabase")
          .Options;

        // Arrange
        var newItem = new InventoryItem { /* Initialize properties */ };

        // Act
        InventoryItem addedItem;
        using (var context = new AppDbContext(_options))
        {
            var service = new InventoryService(context);
            addedItem = await service.AddAsync(newItem);
        }

        // Assert
        Assert.NotNull(addedItem);
        Assert.NotEqual(0, addedItem.Id); // Ensure ID is assigned
    }

    [Fact]
    public async Task UpdateAsync_UpdatesItemCorrectly()
    {
        var _options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDatabase")
          .Options;

        // Arrange
        var updatedItem = new InventoryItem { /* Initialize properties */ };

        using (var context = new AppDbContext(_options))
        {
            context.InventoryItems.Add(updatedItem);
            context.SaveChanges();
        }

        // Modify some properties
        updatedItem.Quantity = 10; // Example change

        // Act
        using (var context = new AppDbContext(_options))
        {
            var service = new InventoryService(context);
            await service.UpdateAsync(updatedItem);
        }

        // Assert
        using (var context = new AppDbContext(_options))
        {
            var retrievedItem = await context.InventoryItems.FindAsync(updatedItem.Id);
            Assert.NotNull(retrievedItem);
            Assert.Equal(10, retrievedItem.Quantity); // Verify the change was persisted
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesItemCorrectly()
    {
        var _options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDatabase")
          .Options;

        // Arrange
        long itemId;
        using (var context = new AppDbContext(_options))
        {
            var itemToDelete = new InventoryItem { /* Initialize properties */ };
            context.InventoryItems.Add(itemToDelete);
            context.SaveChanges();
            itemId = itemToDelete.Id;
        }

        // Act
        using (var context = new AppDbContext(_options))
        {
            var service = new InventoryService(context);
            await service.DeleteAsync(itemId);
        }

        // Assert
        using (var context = new AppDbContext(_options))
        {
            var deletedItem = await context.InventoryItems.FindAsync(itemId);
            Assert.Null(deletedItem);
        }
    }

    public class InventoryItemEqualityComparer : IEqualityComparer<InventoryItem>
    {
        public bool Equals(InventoryItem x, InventoryItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            // Compare properties for equality
            return x.Sku == y.Sku
                && x.Quantity == y.Quantity
                && x.Location == y.Location
                && x.ExpirationDate == y.ExpirationDate
                && x.BatchNumber == y.BatchNumber;
        }

        public int GetHashCode(InventoryItem obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}
