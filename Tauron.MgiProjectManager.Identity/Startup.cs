using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tauron.MgiProjectManager.Identity.Authorization;
using Tauron.MgiProjectManager.Identity.Authorization.Requierments;
using Tauron.MgiProjectManager.Identity.Core;
using Tauron.MgiProjectManager.Identity.Data;
using Tauron.MgiProjectManager.Identity.Services;

namespace Tauron.MgiProjectManager.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();

            Mapper.Initialize(config =>
                              {
                                  config.AddMaps(typeof(IdentityProfile));
                              });

            services.AddSingleton(Mapper.Instance);

            // Configure Identity options and password complexity here
            services.Configure<IdentityOptions>(options =>
                                                {
                                                    // User settings
                                                    options.User.RequireUniqueEmail = true;

                                                    // Password settings
                                                    options.Password.RequireDigit           = false;
                                                    options.Password.RequiredLength         = 5;
                                                    options.Password.RequireNonAlphanumeric = false;
                                                    options.Password.RequireUppercase       = true;
                                                    options.Password.RequireLowercase       = false;

                                                    // Lockout settings
                                                    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(30);
                                                    options.Lockout.MaxFailedAccessAttempts = 10;
                                                });

            string migrationAssembly = typeof(Startup).Assembly.FullName;

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
                                                         options.Authority            = applicationUrl;
                                                         options.SupportedTokens      = SupportedTokens.Jwt;
                                                         options.RequireHttpsMetadata = false; // Note: Set to true in production
                                                         options.ApiName              = IdentityServerConfig.ApiName;
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

            services.AddControllers()
                .AddNewtonsoftJson();

            // Add framework services.
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Latest)
                    .AddFluentValidation(configuration =>
                                         {
                                             configuration.LocalizationEnabled = true;

                                             configuration.RegisterValidatorsFromAssemblyContaining<Startup>();
                                             configuration.RegisterValidatorsFromAssemblyContaining<AccountManager>();
                                             configuration.RegisterValidatorsFromAssemblyContaining<ApplicationRole>();
                                         });

            services.AddTransient<IAccountManager, AccountManager>();

            services.AddSingleton<IAuthorizationHandler, AssignRolesAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ManageUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ViewRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ViewUserAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Configuration.GetSection("Logging"));

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


            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
