using LW.Client.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace LW.Client.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly ILogger<TranslationService> _logger;
        private readonly HttpClient _httpClient;

        public TranslationService(IHttpClientFactory httpClientFactory, ILogger<TranslationService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("lwapi");
            _logger = logger;
        }

        public async Task<TranslationResult> GenerateTranslation(string fromLanguage, string toLanguage, string text)
        {
            try
            {
                var rawContent = new RawTranslation()
                {
                    FromLanguage = fromLanguage,
                    ToLanguage = toLanguage,
                    InputText = text
                };

                var response = await _httpClient.PostAsJsonAsync("translator/generate", rawContent);
                response.EnsureSuccessStatusCode();

                var translationResultJson = await response.Content.ReadAsStringAsync();
                var translationResult = JsonSerializer.Deserialize<TranslationResult>(translationResultJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return translationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating the translation");
                throw;
            }
        }

        public async Task<Dictionary<string, string>> GetLanguages()
        {
            try
            {
                var languagesResponse = await _httpClient.GetAsync("translator/languages/available");

                languagesResponse.EnsureSuccessStatusCode();

                var languagesAvailableJson = await languagesResponse.Content.ReadAsStringAsync();
                var languagesAvailable = JsonSerializer.Deserialize<Dictionary<string, string>>(languagesAvailableJson);

                return languagesAvailable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting the availabe languages");
                throw;
            }
        }
    }
}
