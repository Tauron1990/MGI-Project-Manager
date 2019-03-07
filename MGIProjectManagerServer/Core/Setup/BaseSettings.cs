using System;

namespace MGIProjectManagerServer.Core.Setup
{
    [Serializable]
    public sealed class BaseSettings
    {
        public bool IsConfigurated { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
