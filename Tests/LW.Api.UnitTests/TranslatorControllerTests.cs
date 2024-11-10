using FluentValidation;
using LW.Api.Configs;
using LW.Api.Controllers;
using LW.Api.Models;
using LW.Api.Validators;
using LW.Application.Exceptions;
using LW.Application.Features.Translation;
using LW.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LW.Api.UnitTests
{
    public class Tests
    {
        private Mock<ILogger<TranslatorController>> _loggerMock;
        private Mock<ITranslationService> _translationServiceMock;
        private Mock<IOptions<LanguageSettings>> _languageSettingsMock;
        private IValidator<RawContent> _contentValidator;
        private TranslatorController _controller;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<TranslatorController>>();
            _translationServiceMock = new Mock<ITranslationService>();

            var languageSettings = new LanguageSettings
            {
                SupportedLanguages = new List<Language>
                {
                    new Language { Code = "en-US", Description = "English" },
                    new Language { Code = "fr-FR", Description = "French" }
                }
            };
            _languageSettingsMock = new Mock<IOptions<LanguageSettings>>();
            _languageSettingsMock.Setup(o => o.Value).Returns(languageSettings);

            _contentValidator = new RawContentValidator(_languageSettingsMock.Object);
            _controller = new TranslatorController(
                _loggerMock.Object,
                _translationServiceMock.Object,
                _contentValidator,
                _languageSettingsMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Reset();
            _translationServiceMock.Reset();
            _languageSettingsMock.Reset();
        }

        [Test]
        public async Task When_Create_Translation_Then_Return_Translation_With_Success()
        {
            var rawContent = new RawContent { FromLanguage = "en-US", ToLanguage = "fr-FR", InputText = "Hello" };
            var expectedTranslation = new ContentDto { Language = "fr-FR", Text = "Bonjour" };

            _translationServiceMock.Setup(s => s.GenerateTranslation("en-US", "fr-FR", "Hello"))
                .ReturnsAsync(expectedTranslation);

            var result = await _controller.CreateTranslation(rawContent);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(200, objectResult?.StatusCode);
            Assert.AreEqual(expectedTranslation, objectResult?.Value);
        }

        [Test]
        public void When_Create_Translation_With_InvalidModel_Then_Throws_Exception()
        {
            var rawContent = new RawContent { FromLanguage = "unsupported", ToLanguage = "fr", InputText = "" };
            Assert.ThrowsAsync<ValidationException>(() => _controller.CreateTranslation(rawContent));
        }

        [Test]
        public async Task When_Create_Translation_With_Invalid_TranslationService_Then_Throw_Exception()
        {
            var rawContent = new RawContent { FromLanguage = "en-US", ToLanguage = "fr-FR", InputText = "Hello" };
            _translationServiceMock
                .Setup(s => s.GenerateTranslation("en-US", "fr-FR", "Hello"))
                .ThrowsAsync(new TranslationException("Google API error", "An error occurred while translating the text."));

            Assert.That(() => _controller.CreateTranslation(rawContent), Throws.Exception.TypeOf<TranslationException>());
        }

        [Test]
        public void When_GetAvailableLanguages_Then_Return_Languages()
        {
            var result = _controller.GetAvailableLanguages();

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var languages = okResult?.Value as Dictionary<string, string>;
            Assert.IsNotNull(languages);
            Assert.AreEqual(2, languages?.Count);
            Assert.AreEqual("English", languages?["en-US"]);
            Assert.AreEqual("French", languages?["fr-FR"]);
        }
    }
}