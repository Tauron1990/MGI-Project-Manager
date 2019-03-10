using System;
using System.Collections.Generic;
using System.Text;

namespace Tauron.Application.MgiProjectManager.Server.Data.Core.Setup
{
    [Serializable]
    public sealed class BaseSettings
    {
        private static readonly Dictionary<string, string> Variables = new Dictionary<string, string>
        {
            {"%System%", Environment.GetFolderPath(Environment.SpecialFolder.System)},
            {"%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) },
            {"%Roaming%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) },
            {"%User%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) }
        };

        private static string _cachedPath;

        private string _saveFilePath;

        public bool IsConfigurated { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string SaveFilePath
        {
            get => _saveFilePath;
            set
            {
                _cachedPath = null;
                _saveFilePath = value;
            }
        }

        public string FullSaveFilePath
        {
            get => Expand(_saveFilePath);
        }

        private static string Expand(string path)
        {
            if (_cachedPath != null) return _cachedPath;
            if (!path.StartsWith("%"))
            {
                _cachedPath = path;
                return path;
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(path[0]);

            for (int i = 1; i < path.Length; i++)
            {
                char c = path[i];

                if (c != '%') builder.Append(c);
                else
                {
                    builder.Append(c);
                    break;
                }
            }

            var key = builder.ToString();

            if (!Variables.TryGetValue(key, out var p)) return path;

            _cachedPath = path.Replace(key, p);
            return _cachedPath;
        }
    }
}
