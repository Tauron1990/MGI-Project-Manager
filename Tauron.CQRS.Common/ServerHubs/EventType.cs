namespace Tauron.CQRS.Common.ServerHubs
{
    public enum EventType
    {
        Unkowen,
        Command,
        AmbientCommand,
        Event,
        Query,
        QueryResult
    }
}