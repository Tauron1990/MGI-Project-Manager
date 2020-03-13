using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Engine;
using Tauron.Application.Deployment.Server.Engine.Impl;
using Tauron.Application.OptionsStore;
using Tauron.Application.SimpleAuth;

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
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SimplAuthSettings>(Configuration.GetSection("SimplAuthSettings"));

            services.AddMemoryCache();
            services.AddAuthentication("Simple").AddSimpleAuth();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSimpleAuthApi();

            services.AddDataRaven(Configuration);
            services.AddOptionsStore(s => s.GetRequiredService<IDatabaseCache>().Get("OptionsStore"));

            services.AddSingleton<IAppSetup, AppSetup>();
            services.AddSingleton<DatabaseOptions>();
            services.AddSingleton<IFileSystem, FileSystem>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
