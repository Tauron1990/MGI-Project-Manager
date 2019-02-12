using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGIProjectManagerServer.Core
{
    public static class RoleNames
    {
        public const string Admin = "Admin";

        public const string Controller = "Controller";

        public const string Operator = "Operator";

        public const string Viewer = "Viewer";

        public static IEnumerable<string> GetAllRoles()
        {
            yield return Admin;
            yield return Controller;
            yield return Operator;
            yield return Viewer;
        }

    }
}
