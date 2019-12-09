namespace Tauron.Application.Pipes
{
    public sealed class MessageRecivedEventArgs<TMessage>
    {
        public MessageRecivedEventArgs(TMessage message)
        {
            Message = message;
        }

        public TMessage Message { get; }

        ´
    }
}