using InventoryManagement.API.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Tests.Helpers
{
    public class TestFixture : IDisposable
    {
        public readonly HttpClient Client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public TestFixture()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            Client = _factory.CreateClient();

            RegisterTestUserAsync().Wait(); // Synchronously wait for the async task to complete
        }

        private async Task RegisterTestUserAsync()
        {
            var userLoginRequest = new RegisterDto
            {
                Email = "example@example.com",
                Password = "Pass@#123"
            };

            var jsonContent = JsonConvert.SerializeObject(userLoginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Auth/Register", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to register test user.");
            }


        }

        public void Dispose()
        {
            // Cleanup code if needed
            Client.Dispose();
            _factory.Dispose();
        }
    }
}
