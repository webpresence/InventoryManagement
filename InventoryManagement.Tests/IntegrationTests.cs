using InventoryManagement.API.DTOs;
using InventoryManagement.Tests.Helpers;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
namespace InventoryManagement.Tests
{
    public class IntegrationTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        public IntegrationTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async void AuthControllerTest_Should_Return_Confirmation()
        {
            // Arrange
            var userLoginRequest = new RegisterDto
            {
                Email = "example1@example.com",
                Password = "Pass@#123"
            };
            var jsonContent = JsonConvert.SerializeObject(userLoginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Auth/Register", content);
            var userRegisterResponse = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("User created successfully", userRegisterResponse);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task InventoryItemsController_GetAll_ShouldReturnUnauthorized_When_Token_Not_Present()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/api/InventoryItems");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task InventoryItemsController_GetAll_Should_ReturnArray_When_Token_Present()
        {
            // Arrange
            await AddToken(_client);

            // Act
            var itemsResponse = await _client.GetAsync("/api/InventoryItems");
            var responseContent = await itemsResponse.Content.ReadFromJsonAsync<List<InventoryItemReadDto>>();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, itemsResponse.StatusCode);
            Assert.IsType<List<InventoryItemReadDto>>(responseContent);
        }

        [Fact]
        public async Task GetInventoryItem_ShouldReturnItem_When_ItemExists()
        {
            // Arrange
            long testItemId = 1L;
            await AddToken(_client);

            // Act
            var response = await _client.GetAsync($"/api/InventoryItems/{testItemId}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var item = await response.Content.ReadFromJsonAsync<InventoryItemReadDto>();
            Assert.NotNull(item);
            Assert.Equal(testItemId, item.Id);
        }

        [Fact]
        public async Task GetInventoryItem_ShouldReturnNotFound_When_ItemDoesNotExist()
        {
            // Arrange
            await AddToken(_client);

            // Act
            var response = await _client.GetAsync("/api/InventoryItems/99999");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateInventoryItem_ShouldReturnCreatedItem()
        {
            // Arrange
            await AddToken(_client);
            var createDto = new InventoryItemCreateDto
            {
                BatchNumber = "batch1",
                ExpirationDate = DateTime.Now.AddDays(10),
                Location = "A3",
                Quantity = 1,
                Sku = "abc3"
            };
            var jsonContent = JsonConvert.SerializeObject(createDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/InventoryItems", content);

            // Assert
            var readDto = await response.Content.ReadFromJsonAsync<InventoryItemReadDto>();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(readDto);
            Assert.Equivalent(readDto.Sku, createDto.Sku);
            Assert.Equivalent(readDto.Location, createDto.Location);
            Assert.Equivalent(readDto.ExpirationDate, createDto.ExpirationDate);
            Assert.Equivalent(readDto.Quantity, createDto.Quantity);
        }

        [Fact]
        public async Task InventoryItemsController_Should_Detect_Xss()
        {
            // Arrange
            await AddToken(_client);
            var createDto = new InventoryItemCreateDto
            {
                BatchNumber = @"<script>alert('xss')</script><div onload=""alert('xss')""",
                ExpirationDate = DateTime.Now.AddDays(10),
                Location = "A3",
                Quantity = 1,
                Sku = "abc3"
            };
            var jsonContent = JsonConvert.SerializeObject(createDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/InventoryItems", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateInventoryItem_ShouldReturnNoContent_When_ItemExists()
        {
            // Arrange
            long testItemId = 1L;
            await AddToken(_client);
            var updateDto = new InventoryItemUpdateDto
            {
                BatchNumber = "batch1",
                ExpirationDate = DateTime.Now.AddDays(10),
                Location = "A3",
                Quantity = 1,
                Sku = "abc3"
            };
            var jsonContent = JsonConvert.SerializeObject(updateDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/InventoryItems/{testItemId}", content);

            // Assert
            var itemResponse = await _client.GetAsync($"/api/InventoryItems/{testItemId}");
            var readDto = await itemResponse.Content.ReadFromJsonAsync<InventoryItemReadDto>();
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            Assert.NotNull(readDto);
            Assert.Equivalent(readDto.Sku, updateDto.Sku);
            Assert.Equivalent(readDto.Location, updateDto.Location);
            Assert.Equivalent(readDto.ExpirationDate, updateDto.ExpirationDate);
            Assert.Equivalent(readDto.Quantity, updateDto.Quantity);
        }

        [Fact]
        public async Task UpdateInventoryItem_ShouldReturnUnauthorized_When_Token_Not_Present()
        {
            // Arrange
            long testItemId = 1L;
            var updateDto = new InventoryItemUpdateDto
            {
                BatchNumber = "batch1",
                ExpirationDate = DateTime.Now.AddDays(10),
                Location = "A3",
                Quantity = 1,
                Sku = "abc3"
            };
            var jsonContent = JsonConvert.SerializeObject(updateDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PutAsync($"/api/InventoryItems/{testItemId}", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteInventoryItem_ShouldReturnNoContent_When_ItemExists()
        {
            // Arrange
            long testItemId = 2L;
            await AddToken(_client);

            // Act
            var response = await _client.DeleteAsync($"/api/InventoryItems/{testItemId}");

            // Assert
            var itemResponse = await _client.GetAsync($"/api/InventoryItems/{testItemId}");
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, itemResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteInventoryItem_ShouldReturnUnauthorized_When_Token_Not_Present()
        {
            // Arrange
            long testItemId = 1L;
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.DeleteAsync($"/api/InventoryItems/{testItemId}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<HttpClient> AddToken(HttpClient client)
        {
            // Arrange
            string token = string.Empty;
            var userLoginRequest = new RegisterDto
            {
                Email = "example@example.com",
                Password = "Pass@#123"
            };
            var jsonContent = JsonConvert.SerializeObject(userLoginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Auth/Login", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to login test user.");
            }
            else
            {
                // Assert (within Act for token retrieval)
                var tokenResponse = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(tokenResponse);
                token = json["token"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}
