using System;
using LibGit2Sharp;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public sealed class GitSignature
    {
        public string Name { get; set; } = string.Empty;

        public string EMail { get; set; } = string.Empty;

        public Signature Create() 
            => new Signature(Name, EMail, DateTimeOffset.Now);
    }
}