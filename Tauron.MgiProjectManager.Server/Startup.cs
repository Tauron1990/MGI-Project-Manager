using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Core;
using Tauron.MgiProjectManager.Data.Logging;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Model;
using Tauron.MgiProjectManager.Server.Authorization;
using Tauron.MgiProjectManager.Server.Core;
using Tauron.MgiProjectManager.Server.Hubs;

namespace Tauron.MgiProjectManager.Server
{
    public class Startup
    {
        private IHostingEnvironment Env { get; }
        private IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Env = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            LoggingDbContext.ConnectionString = Configuration.GetConnectionString("LoggingConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<FilesDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("FileConnection")));


            string migrationAssembly = typeof(Startup).Assembly.FullName;

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity options and password complexity here
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;

                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
            });

            // Adds IdentityServer.
            services.AddIdentityServerBuilder()
                .AddCoreServices()
                // The AddDeveloperSigningCredential extension creates temporary key material for signing tokens.
                // This might be useful to get started, but needs to be replaced by some persistent key material for production scenarios.
                // See http://docs.identityserver.io/en/release/topics/crypto.html#refcrypto for more information.
                .AddSigningCredential(new SigningCredentials(new X509SecurityKey(new X509Certificate2(Properties.Resources.ca)), SecurityAlgorithms.Aes256Encryption))
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(migrationAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(migrationAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                })
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);

                //    // this enables automatic token cleanup. this is optional. 
                //    options.EnableTokenCleanup = true;
                //    options.TokenCleanupInterval = 30;
                //})

                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();


            var applicationUrl = Configuration["ApplicationUrl"].TrimEnd('/');

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = applicationUrl;
                    options.SupportedTokens = SupportedTokens.Jwt;
                    options.RequireHttpsMetadata = false; // Note: Set to true in production
                    options.ApiName = IdentityServerConfig.ApiName;
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ViewAllUsersPolicy, policy => policy.RequireClaim(ClaimConstants.Permission, ApplicationPermissions.ViewUsers));
                options.AddPolicy(Policies.ManageAllUsersPolicy, policy => policy.RequireClaim(ClaimConstants.Permission, ApplicationPermissions.ManageUsers));

                options.AddPolicy(Policies.ViewAllRolesPolicy, policy => policy.RequireClaim(ClaimConstants.Permission, ApplicationPermissions.ViewRoles));
                options.AddPolicy(Policies.ViewRoleByRoleNamePolicy, policy => policy.Requirements.Add(new ViewRoleAuthorizationRequirement()));
                options.AddPolicy(Policies.ManageAllRolesPolicy, policy => policy.RequireClaim(ClaimConstants.Permission, ApplicationPermissions.ManageRoles));

                options.AddPolicy(Policies.AssignAllowedRolesPolicy, policy => policy.Requirements.Add(new AssignRolesAuthorizationRequirement()));
                
                options.AddPolicy(Policies.UploadFilesPolicy, builder => builder.RequireClaim(ClaimConstants.Permission, ApplicationPermissions.UploadFiles));
            });


            // Add cors
            services.AddCors();

            // Add framework services.
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(configuration =>
                {
                    configuration.LocalizationEnabled = true;

                    configuration.RegisterValidatorsFromAssemblyContaining<Startup>();
                    configuration.RegisterValidatorsFromAssemblyContaining<AccountManager>();
                    configuration.RegisterValidatorsFromAssemblyContaining<ApplicationRole>();
                });

            // The port to use for https redirection in production
            if (!Env.IsDevelopment() && !string.IsNullOrWhiteSpace(Configuration["HttpsRedirectionPort"]))
            {
                //services.AddHttpsRedirection(options => { options.HttpsPort = int.Parse(Configuration["HttpsRedirectionPort"]); });
            }

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });



            //.AddJsonOptions(opts =>
            //{
            //    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = IdentityServerConfig.ApiFriendlyName, Version = "v1"});
                c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "password",
                    TokenUrl = $"{applicationUrl}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        {IdentityServerConfig.ApiName, IdentityServerConfig.ApiFriendlyName}
                    }
                });

                c.AddFluentValidationRules();
            });

            
            // Configurations
            services.Configure<AppSettings>(Configuration);
            services.AddSignalR(options => options.KeepAliveInterval = TimeSpan.FromMinutes(2));

            //services.AddSingleton(provider =>
            //{
            //    return new Func<IUnitOfWork>(() =>
            //    {
            //        var scope = provider.CreateScope();
            //        return new UnitOfWork(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), scope);
            //    });
            //});
            services.ImportFrom<AccountManager>()
                    .ImportFrom<Startup>()
                    .ImportFrom<OperationManager>()
                    .ConfigurateMapper(typeof(AppSettings));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Configuration.GetSection("Logging"));
            loggerFactory.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Sink(new DatabaseLoggingProvider(loggerFactory))
                .CreateLogger(), true);

            Utilities.ConfigureLogger(loggerFactory);
            EmailTemplates.Initialize(env);

            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("de")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }


            //Configure Cors
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());


            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseIdentityServer();
            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Swagger UI - QuickApp";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{IdentityServerConfig.ApiFriendlyName} V1");
                c.OAuthClientId(IdentityServerConfig.SwaggerClientId);
                c.OAuthClientSecret("no_password"); //Leaving it blank doesn't work
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSignalR(config =>
            {
                config.MapHub<FilesHub>("/Hubs/Files");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    spa.Options.StartupTimeout = TimeSpan.FromSeconds(120); // Increase the timeout if angular app is taking longer to startup
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // Use this instead to use the angular cli server
                }
            });
        }
    }
}
