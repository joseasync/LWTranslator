namespace LW.Application.Exceptions
{
    [Serializable]
    public class TranslationException : Exception
    {
        public string Error { get; set; }

        public TranslationException(string error, string message) : base(message)
        {

            Error = error;

        }
    }
}
