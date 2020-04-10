using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tauron.Application.TooUI.Core;
using Tauron.Application.Wpf.AppCore;

namespace Tauron.Application.TooUI
{
    public sealed class Startup : WpfStartup
    {
        public Startup([NotNull] IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSplash<InternalSplashScreen>();
            services.AddScoped<MainWindowViewModel>();
        }

        public override void Configure(System.Windows.Application app, IHostEnvironment env)
        {
        }
    }
}