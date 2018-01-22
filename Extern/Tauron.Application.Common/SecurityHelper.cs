using System.Security;
using System.Security.Permissions;

namespace Tauron
{
    public static class SecurityHelper
    {
        public static bool IsPermissinGrantedCommon(IPermission permission)
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

        public static bool IsEnvironmentPermissionGranted()
        {
            return IsPermissinGrantedCommon(new EnvironmentPermission(PermissionState.Unrestricted));
        }
    }
}