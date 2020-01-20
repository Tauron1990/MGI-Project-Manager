using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Syncfusion.EJ2.Blazor;
using Syncfusion.Licensing;
using Tauron.Application.Deployment.Server.CoreApp;
using Tauron.Application.Deployment.Server.CoreApp.Services;
using Tauron.Application.OptionsStore;

namespace Tauron.Application.Deployment.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AddServer(services);
            AddClient(services);

            services.AddOptionsStore();
            services.AddSingleton<AppSetup>();
            services.AddSingleton(s => s.GetRequiredService<IConfiguration>().Get<CoreConfig>());
            services.AddSingleton(s => new MongoClient(new MongoUrl(s.GetRequiredService<CoreConfig>().ConnectionString)));
            services.AddSingleton<DatabaseOptions>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();
        }

        private void AddServer(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        private void AddClient(IServiceCollection serviceCollection)
        {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
