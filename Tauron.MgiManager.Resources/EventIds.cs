using Microsoft.Extensions.Logging;

namespace Tauron.MgiManager.Resources
{
    public static class EventIds
    {
        public static class UserManager
        {
            public static EventId UserManagment { get; } = new EventId(1, "UserManagment");
            public static EventId RoleManagment { get; } = new EventId(2, "RoleManagment");
        }
    }
}