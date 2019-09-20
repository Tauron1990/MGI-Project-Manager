using System.Data.SqlClient;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Tauron.CQRS.Health;
using Tauron.CQRS.Server.Extension;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.MgiProjectManager.Dispatcher
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config) => _config = config;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCQRS(c =>
            {
                c.WithDatabase(_config.GetValue<string>("ConnectionString"));
                c.Memory = _config.GetValue<bool>("Memory");
            });

            services.AddHealth();

            services.AddMvc(o => o.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddHealthParts()
                .AddCQRS();
        }

        [UsedImplicitly]
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("C:\\Dispatcher Logs\\Log-{Date}.txt", fileSizeLimitBytes: 100 * 1024 * 1024, retainedFileCountLimit: 5);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.EnableCQRSDevelopmentApiKey();
            }

            app.UseHealth();
            app.UseRouting();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<EventHub>("EventBus");
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/Health");

                    return Task.CompletedTask;
                });
            });
        }
    }
}
