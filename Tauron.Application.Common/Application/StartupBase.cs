using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.Application
{
    public abstract class StartupBase<TApp, THost>
    {
        protected StartupBase(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public abstract void ConfigureServices(IServiceCollection services);

        public abstract void Configure(TApp app, THost env);
    }
}