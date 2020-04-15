using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public class TauronEnviroment : ITauronEnviroment
    {
        public static string AppRepository = "Tauron";

        private string? _defaultPath;

        public string DefaultProfilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultPath))
                    _defaultPath = LocalApplicationData;

                _defaultPath.CreateDirectoryIfNotExis();

                return _defaultPath;
            }

            set => _defaultPath = value;
        }

        public string LocalApplicationData => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath(AppRepository);

        public string LocalApplicationTempFolder => LocalApplicationData.CombinePath("Temp");

        public IEnumerable<string> GetProfiles(string application)
        {
            return
                DefaultProfilePath.CombinePath(application)
                    .EnumerateDirectorys()
                    .Select(ent => ent.Split('\\').Last());
        }
    }
}