using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Tauron.Application.Deployment.Server.Pages.Setup
{
    public static class SetupQueryHelper
    {
        private const string QueryIdName = "SetupID";

        public static string MakeSetupUri(this string uri, string id)
            => QueryHelpers.AddQueryString(uri, QueryIdName, id);

        public static string ReadSetupId(this NavigationManager navManager)
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);

            return QueryHelpers.ParseQuery(uri.Query).TryGetValue(QueryIdName, out var param) ? param.FirstOrDefault() ?? "Invalid" : "Invalid";
        }
    }
}