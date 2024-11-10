using Google;
using Google.Cloud.Translation.V2;
using LW.Application.Exceptions;
using LW.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LW.Application.Features.Translation
{
    public class TranslationService : ITranslationService
    {
        private readonly ILogger<TranslationService> _logger;
        private readonly TranslationClient _translationClient;
        public TranslationService(ILogger<TranslationService> logger, TranslationClient translationClient = null)
        {
            _logger = logger;
            _translationClient = translationClient ?? CreateTranslationClient();
        }

        private TranslationClient CreateTranslationClient()
        {
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_TRANSLATE_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("API key is missing. Set the GOOGLE_TRANSLATE_API_KEY environment variable.");
                throw new TranslationException("API key is missing", "Google Translate API key wasn't set.");
            }

            return TranslationClient.CreateFromApiKey(apiKey);
        }

        public async Task<ContentDto> GenerateTranslation(string fromLanguage, string toLanguage, string text)
        {
            try
            {
                var response = await _translationClient.TranslateTextAsync(text, toLanguage, fromLanguage);

                return new ContentDto
                {
                    Text = response.TranslatedText,
                    Language = toLanguage
                };
            }
            catch (GoogleApiException ex)
            {
                _logger.LogError($"Google API error: {ex.Message}");
                throw new TranslationException("Google API error", "An error occurred while translating the text.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                throw;
            }

        }
    }
}
