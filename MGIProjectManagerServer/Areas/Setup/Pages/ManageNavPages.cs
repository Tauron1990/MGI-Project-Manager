using System;
using System.IO;
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

        public static string StartNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, Start);
        }

        public static string CreateUserNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, CreateUser);
        }

        public static string SetFilePathClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, SetFilePath);
        }

        public static string SetFinishClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, Finish);
        }

        private static string GetActivatePage(ViewContext viewContext)
        {
            return viewContext.ViewData["ActivePage"] as string
                   ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        }

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            return string.Equals(GetActivatePage(viewContext), page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

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