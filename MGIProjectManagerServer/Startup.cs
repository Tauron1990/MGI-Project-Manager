using System.Globalization;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using MGIProjectManagerServer.Core;
using MGIProjectManagerServer.Core.Setup;
using MGIProjectManagerServer.Core.Setup.Impl;
using MGIProjectManagerServer.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Data;
using WebOptimizer;

namespace MGIProjectManagerServer
{
    public class Startup
    {
        public bool IsInDev { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            IsInDev = environment.IsDevelopment();
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzYwNTVAMzEzNjJlMzQyZTMwZ1Q5QUdvdTkxdk5icTNEbFFuZVJ1WGY4cyswWmpaSU5uM094d3p5SWw3QT0=");

            SimpleLoc.SetGlobalResourceManager(WebResources.ResourceManager);

            services.AddWebOptimizer();
            services.Configure<IdentityOptions>(opt => { opt.Password.RequireNonAlphanumeric = false; });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>( o =>
                {
                    o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });

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
                        .RegisterValidatorsFromAssemblyContaining<IndexModel>();
                    fc.LocalizationEnabled = true;
                    fc.ImplicitlyValidateChildProperties = true;
                })
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddAuthentication();
            services.AddAuthorization();

            if (IsInDev)
            {
                services.AddSwaggerDocument();
            }

            services.TryAddTransient<SimpleLoc, SimpleLoc>();
            services.TryAddSingleton<IBaseSettingsManager, BaseSettingsManager>();
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
           var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("de"),
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
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseSwagger();
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseWebOptimizer();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
