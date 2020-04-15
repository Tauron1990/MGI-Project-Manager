using System.Security;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class SecurityHelper
    {
        public static bool IsGranted(this IPermission permission)
        {
            try
            {
                permission.Demand();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }
    }
}