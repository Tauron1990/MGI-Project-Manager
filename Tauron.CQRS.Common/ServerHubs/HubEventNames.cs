namespace Tauron.CQRS.Common.ServerHubs
{
    public static class HubEventNames
    {
        public static class DispatcherEvent
        {
            public const string DeliveryFailedEvent = nameof(DeliveryFailedEvent);
        }

        public static class DispatcherCommand
        {
            public const string StopDispatcher = nameof(StopDispatcher);

            public const string StartDispatcher = nameof(StartDispatcher);
        }

        public static class HeartbeatNames
        {
            public const string Heartbeat = nameof(Heartbeat);

            public const string StillConnected = nameof(StillConnected);
        }

        public static class RejectionReasons
        {
            public const string NoEvent = nameof(NoEvent);

            public const string EventConsumed = nameof(EventConsumed);

            public const string DispatcherStoped = nameof(DispatcherStoped); 
        }

        public const string TryAccept = nameof(TryAccept);

        public const string Subscribe = nameof(Subscribe);

        public const string PropagateEvent = nameof(PropagateEvent);

        public const string AcceptedEvent = nameof(AcceptedEvent);

        public const string RejectedEvent = nameof(RejectedEvent);

        public const string PublishEvent = nameof(PublishEvent);

        public const string PublishEventGroup = nameof(PublishEventGroup);
    }
}