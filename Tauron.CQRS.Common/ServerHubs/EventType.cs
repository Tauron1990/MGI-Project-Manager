namespace Tauron.CQRS.Common.ServerHubs
{
    public enum EventType
    {
        Command,
        TransistentEvent,
        ImportentEvent,
        EssentialEvent
    }
}