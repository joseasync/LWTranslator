using FluentValidation;
using LW.Api.Configs;
using LW.Api.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace LW.Api.Validators
{
    public class RawContentValidator : AbstractValidator<RawContent>
    {
        public RawContentValidator(IOptions<LanguageSettings> languageSettings) {

            var supportedLanguages = languageSettings.Value.SupportedLanguages.Select(lang => lang.Code).ToList();

            RuleFor(c => c.FromLanguage)
                .NotEmpty().WithMessage("The original language is required for this operation.")
                .Must(lang => supportedLanguages.Contains(lang)).WithMessage("Unsupported language.")
                .Must(ValidateBcp47Language).WithMessage("Invalid BCP47 language code.");
            RuleFor(c => c.ToLanguage)
                .NotEmpty().WithMessage("The target language is required for this operation.")
                .Must(lang => supportedLanguages.Contains(lang)).WithMessage("Unsupported language.")
                .Must(ValidateBcp47Language).WithMessage("Invalid BCP47 language code.");
            RuleFor(c => c.InputText)
                .NotEmpty().WithMessage("A text is required for this translation.")
                .Length(0, 500).WithMessage("The text should have 500 characteres at most.");
        }

        private bool ValidateBcp47Language(string languageCode)
        {
            try
            {
                _ = CultureInfo.GetCultureInfo(languageCode);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }

    }
}
