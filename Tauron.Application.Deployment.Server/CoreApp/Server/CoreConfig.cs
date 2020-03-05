﻿namespace Tauron.Application.Deployment.Server.CoreApp.Server
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