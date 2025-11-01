using System.Net;
using System.Text.Json;
using ManagementApp.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ManagementApp.Tests ;

    public class FilialEndpointsTests : IClassFixture<CustomWebAppFactory>
    {
        private readonly HttpClient _client;

        public FilialEndpointsTests(CustomWebAppFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Api-Version", "2.0");
            _client.DefaultRequestHeaders.Add("X-API-Key", "rm555679");
        }

        [Fact]
        public async Task GetAllFiliais_Retorna200EFormatoPaginado()
        {
            var response = await _client.GetAsync("/api/v2/filiais?PageNumber=1&PageSize=2");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = json.RootElement;
            
            Assert.True(root.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array,
                "A resposta não contém uma lista de itens");
            
            Assert.True(items.GetArrayLength() > 0, "A lista de filiais retornada está vazia");
        }

        [Fact]
        public async Task PostFilial_Retorna201ECriaNovaFilial()
        {
            var novaFilial = new
            {
                nome = "Filial Lins",
                cnpj = "93847567000123",
                telefone = "(11) 99999-9999",
                dataAbertura = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                endereco = new
                {
                    cep = "01538-001",
                    logradouro = "Av. Lins de Vasconcelos",
                    numero = "1222",
                    complemento = "Sala 901",
                    bairro = "Aclimação",
                    cidade = "São Paulo",
                    uf = "SP",
                    pais = "Brasil"
                }
            };
            
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v2/filiais")
            {
                Content = JsonContent.Create(novaFilial)
            };
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            
            var response = await _client.SendAsync(request);
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = json.RootElement;
            
            Assert.True(root.TryGetProperty("nome", out var nome));
            Assert.Equal("Filial Lins", nome.GetString());
            
            Assert.True(root.TryGetProperty("cnpj", out var cnpj));
            Assert.Equal("93847567000123", cnpj.GetString());
        }

        [Fact]
        public async Task DeleteFilial_EncerraFilialSemApagar()
        {
            var novaFilial = new
            {
                nome = "Filial Trianon",
                cnpj = "45892345000164",
                telefone = "(11) 88888-8888",
                dataAbertura = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                endereco = new
                {
                    cep = "01311-000",
                    logradouro = "Av. Paulista",
                    numero = "1106",
                    complemento = "7 andar",
                    bairro = "Bela Vista",
                    cidade = "São Paulo",
                    uf = "SP",
                    pais = "Brasil"
                }
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v2/filiais")
            {
                Content = JsonContent.Create(novaFilial)
            };
            postRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            
            var postResponse = await _client.SendAsync(postRequest);
            postResponse.EnsureSuccessStatusCode();
            
            using var postJson = await JsonDocument.ParseAsync(await postResponse.Content.ReadAsStreamAsync());
            var createdId = postJson.RootElement.GetProperty("filialId").GetString();
            
            var deleteResponse = await _client.DeleteAsync($"/api/v2/filiais/{createdId}/encerrar");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            
            using var delJson = await JsonDocument.ParseAsync(await deleteResponse.Content.ReadAsStreamAsync());
            var message = delJson.RootElement.GetProperty("message").GetString();
            Assert.Equal("Filial encerrada com sucesso!", message);
            
            var getResponse = await _client.GetAsync($"/api/v2/filiais/{createdId}");
            getResponse.EnsureSuccessStatusCode();
            
            using var getJson = await JsonDocument.ParseAsync(await getResponse.Content.ReadAsStreamAsync());
            var root = getJson.RootElement;
            
            Assert.True(root.TryGetProperty("dataEncerramento", out var dataEncerramento),
                "A filial não possui DataEncerramento definida após encerramento");
            
            Assert.False(string.IsNullOrEmpty(dataEncerramento.GetString()),
                "DataEncerramento deveria estar preenchida");
        }
    }