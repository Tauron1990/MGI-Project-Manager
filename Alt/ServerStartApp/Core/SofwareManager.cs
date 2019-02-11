using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ServerStartApp.Properties;

namespace ServerStartApp.Core
{
    public static class SofwareManager
    {
        private static Settings _settings = Settings.Default;

        public static void Start(StartType type)
        {
            MessageHelper.SendMessage($"Beginn Start - {type}:");
            switch (type)
            {
                case StartType.PreStart:
                    ExecuteStart(_settings.Prestart);
                    break;
                case StartType.PostStart:
                    ExecuteStart(_settings.PostStart);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void ExecuteStart(string toStart)
        {
            string[] paths = toStart.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(File.Exists).ToArray();

            foreach (var path in paths)
            {
                try
                {
                    var info = new ProcessStartInfo(path);
                    if (!path.EndsWith("exe"))
                        info.UseShellExecute = true;

                    Process.Start(info)?.Dispose();
                    MessageHelper.SendMessage($"\tStartCompled {Path.GetFileName(path)}");
                }
                catch (Exception e)
                {
                    try
                    {
                        MessageHelper.SendMessage($"Error: {e.Message} - {Path.GetFileName(path)}");
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }
    }
}