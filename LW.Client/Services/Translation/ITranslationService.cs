using LW.Client.Models;

namespace LW.Client.Services
{
    public interface ITranslationService
    {
        Task<TranslationResult> GenerateTranslation(string fromLanguage, string toLanguage, string Text);
        Task<Dictionary<string, string>> GetLanguages();
    }
}
