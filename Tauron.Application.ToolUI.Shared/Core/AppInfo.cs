using System;
using System.IO;
using Scrutor;

namespace Tauron.Application.ToolUI.Core
{
    [ServiceDescriptor(typeof(AppInfo))]
    public sealed class AppInfo
    {
        public string SettingsDic { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron", "ToolUI");
    }
}