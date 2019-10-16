using Microsoft.Extensions.Logging;

namespace Tauron.MgiManager.Resources
{
    public static class EventIds
    {
        public static class UserManager
        {
            public static EventId UserCreation { get; } = new EventId(1, "User Creation");
        }
    }
}