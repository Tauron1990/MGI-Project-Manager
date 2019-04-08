using System;
using System.Globalization;
using System.IO;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Syncfusion.Licensing;
using Tauron.Application.MgiProjectManager.BL;
using Tauron.Application.MgiProjectManager.BL.Impl.Hubs;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Core;
using Tauron.Application.MgiProjectManager.Server.Core.Impl;
using Tauron.Application.MgiProjectManager.Server.Core.Setup;
using Tauron.Application.MgiProjectManager.Server.Core.Setup.Impl;
using Tauron.Application.MgiProjectManager.Server.Data;
using Tauron.Application.MgiProjectManager.Server.Data.Api;
using Tauron.Application.MgiProjectManager.Server.Data.Migrations;

namespace Tauron.Application.MgiProjectManager.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            IsInDev = environment.IsDevelopment();
            Configuration = configuration;
        }

        public bool IsInDev { get; set; }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SyncfusionLicenseProvider.RegisterLicense("ODI0MjBAMzEzNzJlMzEyZTMwaURXbU9ZbVdiald4cytvU0RLaURoT3RjSnlrUWdHS2dDa1MzN2hCSEVCWT0=;ODI0MjFAMzEzNzJlMzEyZTM" +
                                                      "wTWh6WU1PUzlBaWhOemJiVzRNMTJZL2gyMTBsSmY1eVk1RVplWkNGYnlnZz0=");

            SimpleLoc.SetGlobalResourceManager(WebResources.ResourceManager);

            services.Configure<IdentityOptions>(opt => { opt.Password.RequireNonAlphanumeric = false; });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(o => { o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")); });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddDefaultUI(UIFramework.Bootstrap4)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(fc =>
                {
                    fc.RegisterValidatorsFromAssemblyContaining<ApplicationDbContext>()
                        .RegisterValidatorsFromAssemblyContaining<Startup>()
                        .RegisterValidatorsFromAssemblyContaining<AppUser>();
                    fc.LocalizationEnabled = true;
                    fc.ImplicitlyValidateChildProperties = true;
                })
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddAuthentication();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(RoleNames.Viewer, builder => { builder.RequireRole(RoleNames.GetAllRoles()); });
                options.AddPolicy(RoleNames.Controller, builder => builder.RequireRole(RoleNames.Controller, RoleNames.Operator, RoleNames.Admin));
                options.AddPolicy(RoleNames.Operator, builder => builder.RequireRole(RoleNames.Operator, RoleNames.Admin));
            });

            if (IsInDev)
            {

                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = "Mgi Project Manager API",
                        Description = "REST API for Mgi Project Manager."
                    });
                });
            }

            services.AddSignalR();
            services.AddAntiforgery();

            RegisterInternalServices(services);
            services.AddDBServices();
            services.AddBLServices();
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/myapp-{Date}.json", isJson: true, retainedFileCountLimit: 10, fileSizeLimitBytes: 10485760);

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("de")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
            if (env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp"),
                    HotModuleReplacement = true
                });
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });

            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseSignalR(builder => builder.MapHub<FilesHub>("/Hubs/Files"));

            app.UseMvc();

            //app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            //{
            //    appBuilder.Run(context => Task.Run(() => context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null));    
            //});
        }

        private void RegisterInternalServices(IServiceCollection services)
        {
            services.TryAddTransient<SimpleLoc, SimpleLoc>();
            services.TryAddSingleton<IBaseSettingsManager, BaseSettingsManager>();
            services.AddTransient<Func<ApplicationDbContext>>(provider =>
            {
                return () =>
                {
                    var scoper = provider.CreateScope();

                    var context = scoper.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.CurrentScope = scoper;
                    return context;
                };
            });
            services.AddSingleton(Configuration);
            services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
        }
    }
}
