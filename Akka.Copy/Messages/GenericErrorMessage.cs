namespace Akka.Copy.Messages
{
    public sealed class GenericErrorMessage
    {
        public string Message { get; }

        public GenericErrorMessage(string message) => Message = message;
    }
}