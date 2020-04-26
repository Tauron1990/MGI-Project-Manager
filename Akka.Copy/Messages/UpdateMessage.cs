namespace Akka.Copy.Messages
{
    public sealed class UpdateMessage
    {
        public string Message { get; }

        public double Percent { get; }

        public UpdateMessage(string message, double percent)
        {
            Message = message;
            Percent = percent;
        }
    }
}