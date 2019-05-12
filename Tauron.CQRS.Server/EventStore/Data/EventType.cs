namespace Tauron.CQRS.Server.EventStore.Data
{
    public enum EventType
    {
        Command,
        TransistentEvent,
        ImportentEvent,
        EssentialEvent
    }
}