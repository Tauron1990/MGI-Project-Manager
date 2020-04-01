using System;
using System.IO;
using System.Threading;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Neleus.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.Deployment.Server.Engine;
using Tauron.Application.Deployment.Server.Engine.Impl;
using Tauron.Application.Deployment.Server.Engine.Provider;
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
            services.Configure<LocalSettings>(Configuration);

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

            services.AddByName<IRepoProvider, RepositoryProvider>()
               .Build();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddHostedService<SyncService>();
            services.AddSingleton<DatabaseOptions>();
            services.AddTransient<IFileSystem, FileSystem>();

            services.AddControllers();
            services.AddSwaggerGen(o =>
                                   {
                                       o.SwaggerDoc("TauronDeploymentServer", new OpenApiInfo
                                                                              {
                                                                                  Title = "Tauron Deployment Server",
                                                                                  Version = "v1"
                                                                              });

                                       o.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "Tauron.Application.SimpleAuth.xml"));
                                       o.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "Tauron.Application.Deployment.Server.xml"));
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