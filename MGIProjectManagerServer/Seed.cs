using System;
using System.IO;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.MgiProjectManager.Server.Data;

namespace MGIProjectManagerServer
{
    public static class Seed
    {
        public static void CreateRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            string userName = configuration.GetSection("UserSettings")["UserEmail"];
            string userPassword = configuration.GetSection("UserSettings")["UserPassword"];


            //adding custom roles
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var env = serviceProvider.GetRequiredService<IHostingEnvironment>();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron\\MGiProjectManager\\db");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

             ApplicationDbContext.ConnectionPath = Path.Combine(path, "mgi.db");

            using (var context = new ApplicationDbContext())
                context.Database.Migrate();

             foreach (var roleName in RoleNames.GetAllRoles())
            {
                var roleExist = roleManager.RoleExistsAsync(roleName).Result;
                if (!roleExist)
                    roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
            }

            var poweruser = new IdentityUser
            {
                UserName = userName,
                Email = userName
            };

            var user = userManager.FindByEmailAsync(userName).Result;

            if (user != null) return;

            var createPowerUser = userManager.CreateAsync(poweruser, userPassword).Result;
            if (createPowerUser.Succeeded)
            {
                userManager.AddToRoleAsync(poweruser, roleNames[0]).Wait();
            }
        }
    }
}