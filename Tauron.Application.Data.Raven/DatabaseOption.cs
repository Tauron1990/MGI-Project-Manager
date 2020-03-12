using System;

namespace Tauron.Application.Data.Raven
{
    public sealed class DatabaseOption
    {
        public string[] Urls { get; set; } = Array.Empty<string>();

        public bool Debug { get; set; } = false;
    }
}