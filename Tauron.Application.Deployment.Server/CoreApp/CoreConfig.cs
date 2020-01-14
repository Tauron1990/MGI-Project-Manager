using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tauron.Application.Deployment.Server.CoreApp
{
    public class CoreConfig
    {
        public string ConnectionString { get; }

        public CoreConfig(string connectionString) 
            => ConnectionString = connectionString;
    }
}
