using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.EJ2.Blazor;
using Syncfusion.Licensing;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.CoreApp.Bridge;
using Tauron.Application.Deployment.Server.CoreApp.Bridge.Impl;
using Tauron.Application.Deployment.Server.CoreApp.Client;
using Tauron.Application.Deployment.Server.CoreApp.Client.Impl;
using Tauron.Application.Deployment.Server.CoreApp.Server;
using Tauron.Application.Deployment.Server.CoreApp.Server.Impl;
using Tauron.Application.OptionsStore;

namespace Tauron.Application.Deployment.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AddServer(services);
            AddClient(services);



            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();
        }

        private void AddServer(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            serviceCollection.AddDataRaven(Configuration);
            serviceCollection.AddOptionsStore(s => s.GetRequiredService<IDatabaseCache>().Get("OptionsStore"));
            serviceCollection.AddSingleton<IAppSetup, AppSetup>();
            serviceCollection.AddSingleton(s => s.GetRequiredService<IConfiguration>().Get<CoreConfig>());
            serviceCollection.AddSingleton<DatabaseOptions>();
            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
        }

        private void AddClient(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServerBridge, ServerBridgeImpl>();
            serviceCollection.AddSingleton<IErrorDelegator, ErrorDelegator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            SyncfusionLicenseProvider.RegisterLicense("MTk4MTg2QDMxMzcyZTM0MmUzMG5iUXVzNVdGci9ERVNhOW40WG00ZmZnRnRXMTNrbVY3Y0hxKzBEVE50bms9");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            UseServer(app);
            UseClient(app);

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void UseServer(IApplicationBuilder app)
        {
            app.UseCors();
        }

        private void UseClient(IApplicationBuilder app)
        {

        }
    }
}