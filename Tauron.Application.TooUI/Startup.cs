using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Logging;
using Tauron.Application.ToolUI.Core;
using Tauron.Application.Wpf.AppCore;

namespace Tauron.Application.ToolUI
{
    public sealed class Startup : WpfStartup
    {
        public Startup([NotNull] IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSplash<InternalSplashScreen>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddScoped<IToolSwitcher, ToolSwitcher>();
            services.AddTransient(s => Log.Logger);
            services.AddSingleton(s => Settings.Create(s.GetRequiredService<AppInfo>()));

            services.AddTauronLogging();
            services.AddSoftwareRepo();
        }

        public override void Configure(System.Windows.Application app, IHostEnvironment env)
        {
        }
    }
}