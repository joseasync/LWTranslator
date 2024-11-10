using LW.Application.Features.Translation;

namespace LW.Application.Interfaces
{
    public interface ITranslationService
    {
        Task<ContentDto> GenerateTranslation(string fromLanguage, string toLanguage, string Text);
    }
}
