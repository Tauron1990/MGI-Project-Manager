using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MgiManager.Areas.Identity;
using MgiManager.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Tauron.CQRS.Services.Extensions;

namespace MgiManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration) 
            => Configuration = configuration;

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddCQRSServices(config =>
                                     {
                                         config.SetUrls(new Uri(Configuration.GetValue<string>("Dispatcher")),
                                                        Configuration.GetValue<string>("ServiceName"),
                                                        Configuration.GetValue<string>("ApiKey"))
                                            .AddFrom<Startup>();
                                     });
            
            services.AddIdentity<InternalUser, InternalRole>(options =>
                                                             {
                                                                 var pass = options.Password;
                                                                 pass.RequiredUniqueChars = 0;
                                                                 pass.RequireDigit = false;
                                                                 pass.RequireLowercase = false;
                                                                 pass.RequireNonAlphanumeric = false;
                                                                 pass.RequireUppercase = false;
                                                                 pass.RequiredLength = 3;
                                                             })
               .AddDefaultTokenProviders().AddDefaultUI();

            services.AddTransient<IUserStore<InternalUser>, InternalUserStore>();
            services.AddTransient<IRoleStore<InternalRole>, InternalRoleStore>();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.StartCQRS().GetAwaiter().GetResult();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
