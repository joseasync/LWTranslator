using FluentValidation;
using LW.Api.Configs;
using LW.Api.Models;
using LW.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LW.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class TranslatorController : ControllerBase
    {
        private readonly ILogger<TranslatorController> _logger;
        private readonly IValidator<RawContent> _contentValidator;
        private readonly ITranslationService _service;
        private readonly LanguageSettings _languageSettings;

        public TranslatorController(
            ILogger<TranslatorController> logger,
            ITranslationService service,
            IValidator<RawContent> contentValidator,
            IOptions<LanguageSettings> languageSettings)
        {
            _logger = logger;
            _service = service;
            _contentValidator = contentValidator;
            _languageSettings = languageSettings.Value;
        }


        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpPost("translator/generate")]
        public async Task<IActionResult> CreateTranslation([FromBody] RawContent rawContent)
        {
            _contentValidator.ValidateAndThrow(rawContent);
            var translationResult = await _service.GenerateTranslation(rawContent.FromLanguage, rawContent.ToLanguage,
                rawContent.InputText);

            return Ok(translationResult);
        }

        [ProducesResponseType(200)]
        [HttpGet("translator/languages/available")]
        public IActionResult GetAvailableLanguages()
        {
            var languages = _languageSettings.SupportedLanguages.ToDictionary(
                lang => lang.Code,
                lang => lang.Description
            );
            return Ok(languages);
        }
    }
}
