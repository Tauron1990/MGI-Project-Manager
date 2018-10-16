using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace Tauron.Application.Models.Rules
{
    internal static class ResourceMessages
    {
        private static ResourceManager _resourceManager;

        public static ResourceManager ResourceManager => _resourceManager ??
                                                         (_resourceManager =
                                                             new ResourceManager(
                                                                 "System.ComponentModel.DataAnnotations.Resources.DataAnnotationsResources",
                                                                 typeof(Validator).Assembly));

        internal static string RequireRuleError => ResourceManager.GetString("RequiredAttribute_ValidationError");
    }
}