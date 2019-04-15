// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Core;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Model;

namespace Tauron.MgiProjectManager.Data
{
    [Export(typeof(IDatabaseInitializer), LiveCycle = LiveCycle.Transistent)]
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;
        private readonly ILogger<DatabaseInitializer> _logger;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly ConfigurationDbContext _configurationDbContext;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager, ILogger<DatabaseInitializer> logger, PersistedGrantDbContext persistedGrantDbContext,
            ConfigurationDbContext configurationDbContext, ITimedTaskManager timedTaskManager)
        {
            timedTaskManager.Start();
            _accountManager = accountManager;
            _context = context;
            _logger = logger;
            _persistedGrantDbContext = persistedGrantDbContext;
            _configurationDbContext = configurationDbContext;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync().ConfigureAwait(false);

            if (!await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Generating inbuilt accounts");

                const string adminRoleName = "administrator";
                //const string userRoleName = "user";

                await EnsureRoleAsync(adminRoleName, "Administrator", ApplicationPermissions.GetAllPermissionValues());
                //await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                await CreateUserAsync("Admin", "Admin", "Inbuilt Administrator", "admin@ebenmonney.com", "+1 (123) 000-0000", new [] {adminRoleName});
                //await CreateUserAsync("user", "tempP@ss123", "Inbuilt Standard User", "user@ebenmonney.com", "+1 (123) 000-0001", new [] {userRoleName});

                _logger.LogInformation("Inbuilt account generation completed");
            }

            await _context.SaveChangesAsync();

            await _persistedGrantDbContext.Database.MigrateAsync();
            await _configurationDbContext.Database.MigrateAsync();
            
            if (!await _configurationDbContext.Clients.AnyAsync())
                await _configurationDbContext.Clients.AddRangeAsync(IdentityServerConfig.GetClients().Select(m => m.ToEntity()));

            if (!await _configurationDbContext.IdentityResources.AnyAsync())
                await _configurationDbContext.IdentityResources.AddRangeAsync(IdentityServerConfig.GetIdentityResources().Select(m => m.ToEntity()));

            if (!await _configurationDbContext.ApiResources.AnyAsync())
                await _configurationDbContext.ApiResources.AddRangeAsync(IdentityServerConfig.GetApiResources().Select(m => m.ToEntity()));

            await _configurationDbContext.SaveChangesAsync();

            _logger.LogInformation("Seeding initial data completed");

        }



        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                var result = await _accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Succeeded)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }
        }

        private async Task CreateUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            var result = await _accountManager.CreateUserAsync(applicationUser, roles, password);

            if (!result.Succeeded)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
        }
    }
}
