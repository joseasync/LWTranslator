namespace LW.Api.Configs
{
    public class Language
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class LanguageSettings
    {
        public List<Language> SupportedLanguages { get; set; }
    }
}
