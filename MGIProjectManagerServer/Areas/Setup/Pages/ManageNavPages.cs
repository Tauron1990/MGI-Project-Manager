using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MGIProjectManagerServer.Areas.Setup.Pages
{
    public static class SetupManageNavPages
    {
        public const string LabelName = "NextLabel";

        public static string Start => "Start";

        public static string CreateUser => "CreateUser";

        public static string SetFilePath => "SetFilePath";

        public static string Finish => "Finish";

        public static string StartNavClass(ViewContext viewContext) => PageNavClass(viewContext, Start);

        public static string CreateUserNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateUser);

        public static string SetFilePathClass(ViewContext viewContext) => PageNavClass(viewContext, SetFilePath);

        public static string SetFinishClass(ViewContext viewContext) => PageNavClass(viewContext, Finish);

        private static string GetActivatePage(ViewContext viewContext) =>
            viewContext.ViewData["ActivePage"] as string
            ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

        private static string PageNavClass(ViewContext viewContext, string page) 
            => string.Equals(GetActivatePage(viewContext), page, StringComparison.OrdinalIgnoreCase) ? "active" : null;

        public static string GetNext(ViewContext viewContext)
        {
            switch (GetActivatePage(viewContext))
            {
                case "Finish":
                    return "/Finish";
                case "CreateUser":
                    return "/CreateUser";
                case "Start":
                    return "/Start";
                default:
                    return "/Index";
            }
        }

        public static string SelectArea(ViewContext context)
        {
            switch (GetActivatePage(context))
            {
                default:
                    return "Setup";
            }
        }

        public static string GetHandler(ViewContext context)
        {
            switch (GetActivatePage(context))
            {
                case "Finish":
                    return "Next";
                case "CreateUser":
                    return "Next";
                case "Start":
                    return "Next";
                default:
                    return null;
            }
        }
    }
}