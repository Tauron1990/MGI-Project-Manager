namespace Tauron.CQRS.Common.ServerHubs
{
    public enum EventType
    {
        Unkowen,
        Command,
        TransistentEvent,
        Query,
        QueryResult
    }
}