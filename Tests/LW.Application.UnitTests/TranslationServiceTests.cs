using Google;
using Google.Cloud.Translation.V2;
using LW.Application.Exceptions;
using LW.Application.Features.Translation;
using Microsoft.Extensions.Logging;
using Moq;

namespace LW.Application.UnitTests
{
    public class TranslationServiceTests
    {
        private Mock<ILogger<TranslationService>> _loggerMock;
        private Mock<TranslationClient> _translationClientMock;
        private TranslationService _service;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<TranslationService>>();
            _translationClientMock = new Mock<TranslationClient>();
            _service = new TranslationService(_loggerMock.Object, _translationClientMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _loggerMock.Reset();
            _translationClientMock.Reset();
        }

        [Test]
        public async Task When_GenerateTranslation_With_ValidInput_Then_ReturnsTranslatedText()
        {
            var fromLanguage = "en-US";
            var toLanguage = "fr-FR";
            var inputText = "Hello";
            var translatedText = "Bonjour";
            var translationResponse = new TranslationResult(inputText, translatedText, fromLanguage, fromLanguage,toLanguage,null);

            _translationClientMock.Setup(client => client.TranslateTextAsync(inputText, toLanguage, fromLanguage, null, default))
                .ReturnsAsync(translationResponse);

            var result = await _service.GenerateTranslation(fromLanguage, toLanguage, inputText);

            Assert.IsNotNull(result);
            Assert.AreEqual(translatedText, result.Text);
            Assert.AreEqual(toLanguage, result.Language);
        }

        [Test]
        public void When_GenerateTranslation_With_MissingApiKey_Then_ThrowsTranslationException()
        {
            Environment.SetEnvironmentVariable("GOOGLE_TRANSLATE_API_KEY", null);
            Assert.Throws<TranslationException>(() => new TranslationService(_loggerMock.Object));
        }

        [Test]
        public void When_GenerateTranslation_With_GoogleApiError_Then_ThrowsTranslationException()
        {
            var fromLanguage = "en-US";
            var toLanguage = "fr-FR";
            var inputText = "Hello";

            _translationClientMock.Setup(client => client.TranslateTextAsync(inputText, toLanguage, fromLanguage, null, default))
                .ThrowsAsync(new GoogleApiException("An error occurred while translating the text.", null));

            var ex = Assert.ThrowsAsync<TranslationException>(
                () => _service.GenerateTranslation(fromLanguage, toLanguage, inputText));
            Assert.AreEqual("An error occurred while translating the text.", ex.Message);
        }

        [Test]
        public void When_GenerateTranslation_With_UnexpectedError_Then_ThrowsException()
        {
            var fromLanguage = "en-US";
            var toLanguage = "fr-FR";
            var inputText = "Hello";

            _translationClientMock.Setup(client => client.TranslateTextAsync(inputText, toLanguage, fromLanguage, null, default))
                .ThrowsAsync(new Exception("Unexpected error"));

            var ex = Assert.ThrowsAsync<Exception>(() => _service.GenerateTranslation(fromLanguage, toLanguage, inputText));
            Assert.AreEqual("Unexpected error", ex.Message);
        }

    }
}