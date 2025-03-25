using System.Net.Http.Json;

namespace PersistenciaService.Services.ViaCep
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ViaCepService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ViaCepResponse?> ConsultarCepAsync(string cep)
        {
            var baseUrl = _configuration["ApiSettings:ViaCepUrl"];
            var response = await _httpClient.GetAsync($"{baseUrl}{cep}/json");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ViaCepResponse>();
        }
    }
}
