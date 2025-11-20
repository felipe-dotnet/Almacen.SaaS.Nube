using Newtonsoft.Json;
using System.Text;

namespace Almacen.Saas.API.Test;
public class AuthControllerTest:IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public AuthControllerTest(IntegrationTestFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Login_With_ValidCredentials_ReturnsTokens()
    {
        // Definir el payload con usuario de prueba (asegúrate que existe en la base)
        var payload = new
        {
            Email = "usuario@prueba.com",
            Password = "123456"
        };

        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Realiza la petición al endpoint de login
        var response = await _client.PostAsync("/api/auth/login", content);

        // Verifica que fue exitosa
        response.EnsureSuccessStatusCode();

        // Verifica que el resultado contiene los tokens
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("accessToken", responseContent);
        Assert.Contains("refreshToken", responseContent);
    }
}
