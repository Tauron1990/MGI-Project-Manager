using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Raven.Client.Documents;
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
            services.AddOptionsStore(() => new DocumentStore
                                          {
                                              Urls = new[] {"http://127.0.0.1:8080/"},
                                              Database = "Options_Store"
                                          }.Initialize());
            services.AddSingleton<AppSetup>();
            services.AddSingleton(s => s.GetRequiredService<IConfiguration>().Get<CoreConfig>());
            services.AddSingleton<DatabaseOptions>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSyncfusionBlazor();
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

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
