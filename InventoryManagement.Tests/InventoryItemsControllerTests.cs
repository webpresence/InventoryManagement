using AutoMapper;
using InventoryManagement.API.Controllers;
using InventoryManagement.API.DTOs;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace InventoryManagement.Tests
{
    public class InventoryItemsControllerTests
    {
        [Fact]
        public async Task GetAllInventoryItems_ReturnsAllItems()
        {
            // Arrange
            var mockService = new Mock<IInventoryService>();
            var mockMapper = new Mock<IMapper>();
            var items = new List<InventoryItem>
        {
            new InventoryItem { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" },
            new InventoryItem { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" }
        };
            mockService.Setup(service => service.GetAllAsync()).ReturnsAsync(items);
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<InventoryItemReadDto>>(It.IsAny<IEnumerable<InventoryItem>>()))
                       .Returns(items.Select(item => new InventoryItemReadDto { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" }));

            var controller = new InventoryItemsController(mockService.Object, mockMapper.Object);

            // Act
            var result = await controller.GetAllInventoryItems();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<InventoryItemReadDto>>(actionResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetInventoryItem_ReturnsItem_WhenItemExists()
        {
            // Arrange
            var mockService = new Mock<IInventoryService>();
            var mockMapper = new Mock<IMapper>();
            var itemId = 1L;
            var inventoryItem = new InventoryItem { Id = itemId };
            mockService.Setup(service => service.GetByIdAsync(itemId)).ReturnsAsync(inventoryItem);
            mockMapper.Setup(mapper => mapper.Map<InventoryItemReadDto>(It.IsAny<InventoryItem>()))
                      .Returns(new InventoryItemReadDto { Id = itemId });

            var controller = new InventoryItemsController(mockService.Object, mockMapper.Object);

            // Act
            var result = await controller.GetInventoryItem(itemId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<InventoryItemReadDto>(actionResult.Value);
            Assert.Equal(itemId, model.Id);
        }

        [Fact]
        public async Task CreateInventoryItem_ReturnsCreatedAtActionResult_WithInventoryItem()
        {
            // Arrange
            var mockService = new Mock<IInventoryService>();
            var mockMapper = new Mock<IMapper>();
            var createDto = new InventoryItemCreateDto { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" };
            var inventoryItem = new InventoryItem { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" };
            var readDto = new InventoryItemReadDto { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" };

            mockMapper.Setup(m => m.Map<InventoryItem>(It.IsAny<InventoryItemCreateDto>())).Returns(inventoryItem);
            mockService.Setup(s => s.AddAsync(It.IsAny<InventoryItem>())).ReturnsAsync(inventoryItem);
            mockMapper.Setup(m => m.Map<InventoryItemReadDto>(It.IsAny<InventoryItem>())).Returns(readDto);

            var controller = new InventoryItemsController(mockService.Object, mockMapper.Object);

            // Act
            var result = await controller.CreateInventoryItem(createDto);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetInventoryItem", actionResult.ActionName);
            Assert.IsType<InventoryItemReadDto>(actionResult.Value);
            var dtoResult = actionResult.Value as InventoryItemReadDto;
            // Assert further on dtoResult properties if necessary
        }

        [Fact]
        public async Task UpdateInventoryItem_ReturnsNoContent_WhenItemExists()
        {
            // Arrange
            var mockService = new Mock<IInventoryService>();
            var mockMapper = new Mock<IMapper>();
            long itemId = 1;
            var updateDto = new InventoryItemUpdateDto { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" };
            var inventoryItem = new InventoryItem { BatchNumber = "123", ExpirationDate = DateTime.Now.AddDays(1), Location = "B1", Quantity = 99, Sku = "sku1" };

            mockService.Setup(s => s.GetByIdAsync(itemId)).ReturnsAsync(inventoryItem);
            mockService.Setup(s => s.UpdateAsync(It.IsAny<InventoryItem>())).Returns(Task.CompletedTask);
            mockMapper.Setup(m => m.Map(It.IsAny<InventoryItemUpdateDto>(), It.IsAny<InventoryItem>()));

            var controller = new InventoryItemsController(mockService.Object, mockMapper.Object);

            // Act
            var result = await controller.UpdateInventoryItem(itemId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateInventoryItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<IInventoryService>();
            var mockMapper = new Mock<IMapper>();
            long itemId = 1;
            var updateDto = new InventoryItemUpdateDto();

            mockService.Setup(s => s.GetByIdAsync(itemId)).ReturnsAsync((InventoryItem)null);

            var controller = new InventoryItemsController(mockService.Object, mockMapper.Object);

            // Act
            var result = await controller.UpdateInventoryItem(itemId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task DeleteInventoryItem_ReturnsNoContent_WhenItemExists()
        {
            // Arrange
            var mockService = new Mock<IInventoryService>();
            long itemId = 1;

            mockService.Setup(s => s.DeleteAsync(itemId)).Returns(Task.CompletedTask);
            mockService.Setup(s => s.GetByIdAsync(itemId)).ReturnsAsync(new InventoryItem { Id = itemId });

            var controller = new InventoryItemsController(mockService.Object, null);

            // Act
            var result = await controller.DeleteInventoryItem(itemId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteInventoryItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<IInventoryService>();
            long itemId = 1;

            mockService.Setup(s => s.GetByIdAsync(itemId)).ReturnsAsync((InventoryItem)null);

            var controller = new InventoryItemsController(mockService.Object, null);

            // Act
            var result = await controller.DeleteInventoryItem(itemId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
