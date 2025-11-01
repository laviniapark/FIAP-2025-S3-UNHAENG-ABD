using System.Net;
using Xunit;

namespace ManagementApp.Tests ;

    public class PredictMaintenanceEndpointTest : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public PredictMaintenanceEndpointTest(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Api-Version", "2.0");
            _client.DefaultRequestHeaders.Add("X-API-Key", "rm555679");
        }
        
        [Fact]
        public async Task PredictMaintenance_RetornaCustoPrevisto()
        {
            var input = new
            {
                quilometragem = 35000,
                anosUso = 3
            };

            var response = await _client.PostAsJsonAsync("/api/v2/predict-manutencao", input);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            using var json = await System.Text.Json.JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = json.RootElement;

            Assert.True(root.TryGetProperty("custoPrevisto", out var custoPrevisto),
                "A resposta não contém o campo 'custoPrevisto'");
            
            var valor = custoPrevisto.GetDouble();
            Assert.InRange(valor, 0, 5000);
        }
    }