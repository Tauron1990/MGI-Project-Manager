using System;
using System.IO;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Engine;
using Tauron.Application.Deployment.Server.Engine.Impl;
using Tauron.Application.Logging;
using Tauron.Application.OptionsStore;
using Tauron.Application.SimpleAuth;
using Tauron.Application.SimpleAuth.Core;

namespace Tauron.Application.Deployment.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SimplAuthSettings>(Configuration.GetSection("SimplAuthSettings"));

            services.AddHttpContextAccessor();

            services.AddTauronLogging();
            services.AddMemoryCache();
            services.AddAuthentication("Simple").AddSimpleAuth();
            services.AddMvc()
               .AddFluentValidation(c =>
                                    {
                                        c.RegisterValidatorsFromAssemblyContaining<Startup>();
                                        c.RegisterValidatorsFromAssemblyContaining<SimpleAuthenticationOptions>();
                                    })
               .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
               .AddSimpleAuthApi();

            services.AddDataRaven(Configuration);
            services.AddOptionsStore(s => s.GetRequiredService<IDatabaseCache>().Get("OptionsStore"));

            services.AddSingleton<IAppSetup, AppSetup>();
            services.AddSingleton<DatabaseOptions>();
            services.AddSingleton<IFileSystem, FileSystem>();

            services.AddControllers();
            services.AddSwaggerGen(o =>
                                   {
                                       o.SwaggerDoc("TauronDeploymentServer", new OpenApiInfo
                                                                              {
                                                                                  Title = "Tauron Deployment Server",
                                                                                  Version = "v1"
                                                                              });

                                       o.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "Tauron.Application.SimpleAuth.xml"));
                                       o.AddFluentValidationRules();
                                   });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();


            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger().UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/TauronDeploymentServer/swagger.json", "Tauron Deployment Server"); });
        }
    }
}