using System;
using System.Globalization;
using System.Resources;

namespace MGIProjectManagerServer.Core
{
    public sealed class SimpleLoc
    {
        private static ResourceManager _resourceManager;

        public static void SetGlobalResourceManager(ResourceManager manager)
        {
            if(_resourceManager != null) 
                throw new InvalidOperationException();

            _resourceManager = manager;
        }

        public string this[string key] => _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

        public string this[string key, params object[] elemnts] 
            => elemnts.Length == 0 
                ? _resourceManager.GetString(key) 
                : string.Format(_resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key, elemnts);
    }
}