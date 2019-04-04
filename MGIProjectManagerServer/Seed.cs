using System;
using MGIProjectManagerServer.Core.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Server.Data;

namespace MGIProjectManagerServer
{
    public static class Seed
    {
        //public static string UserPassword { get; private set; }

        //public static string UserName { get; private set; }

        public static void CreateRoles(IServiceProvider serviceProvider, IConfiguration configuration, ITimedTaskManager timedTaskManager)
        {
            timedTaskManager.Start();

            using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                context.Database.Migrate();
            }

            var manager = serviceProvider.GetRequiredService<IBaseSettingsManager>();
            manager.Read();

            if (manager.BaseSettings.IsConfigurated) return;
            var settings = manager.BaseSettings;

            settings.UserName = configuration.GetSection("UserSettings")["UserEmail"];
            settings.Password = configuration.GetSection("UserSettings")["UserPassword"];
            settings.SaveFilePath = @"%Roaming%\Tauron\MGIManager";

            //
            //var env = serviceProvider.GetRequiredService<IHostingEnvironment>();

            //adding custom roles
            //var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            //var env = serviceProvider.GetRequiredService<IHostingEnvironment>();

            //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron\\MGiProjectManager\\db");
            //if (!Directory.Exists(path))
            //    Directory.CreateDirectory(path);

            //ApplicationDbContext.ConnectionPath = Path.Combine(path, "mgi.db");

            //foreach (var roleName in RoleNames.GetAllRoles())
            //{
            //    var roleExist = roleManager.RoleExistsAsync(roleName).Result;
            //    if (!roleExist)
            //        roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
            //}

            //var poweruser = new IdentityUser
            //{
            //    UserName = UserName,
            //    Email = UserName
            //};

            //var user = userManager.FindByEmailAsync(UserName).Result;

            //if (user != null) return;

            //var createPowerUser = userManager.CreateAsync(poweruser, UserPassword).Result;
            //if (createPowerUser.Succeeded)
            //{
            //    userManager.AddToRoleAsync(poweruser, RoleNames.Admin).Wait();
            //}
        }
    }
}