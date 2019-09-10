using System;
using JetBrains.Annotations;

namespace Tauron.CQRS.Common.Configuration
{
    public class ServerConfiguration : CommonConfiguration
    {
        public string ConnectionString { get; set; }

        public bool Memory { get; set; }

        [PublicAPI]
        public ServerConfiguration WithDatabase(string connectionString)
        {
            ConnectionString = connectionString;

            return this;
        }
    }
}