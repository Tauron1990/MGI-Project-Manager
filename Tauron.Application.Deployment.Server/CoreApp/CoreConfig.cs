using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tauron.Application.Deployment.Server.CoreApp
{
    public class CoreConfig
    {
        public string ConnectionString { get; set; } = string.Empty;

        public CoreConfig(string connectionString) 
            => ConnectionString = connectionString;

        public CoreConfig()
        {
            
        }
    }
}
