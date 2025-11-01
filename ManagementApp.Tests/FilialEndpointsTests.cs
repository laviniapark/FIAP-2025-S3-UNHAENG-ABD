using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ManagementApp.Tests ;

    public class FilialEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public FilialEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Api-Version", "2.0");
        }

        [Fact]
        public async Task GetFiliais_RetornaListaDeFiliais()
        {
            var response = await _client.GetAsync("/api/v2/filiais");

            response.EnsureSuccessStatusCode();
            var filiais = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(filiais));
        }
    }