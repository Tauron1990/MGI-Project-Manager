using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server.Core.Impl
{
    public class ApiKeyStore : IApiKeyStore
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ApiKeyStore> _logger;
        private readonly IConfiguration _configuration;

        private readonly Lazy<HashAlgorithm> _hashAlgorithm =
            new Lazy<HashAlgorithm>(() => HashAlgorithm.Create("sha256"));

        private readonly List<ApiKey> _keys = new List<ApiKey>();
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private bool _isInit;

        public ApiKeyStore(IServiceScopeFactory serviceScopeFactory, ILogger<ApiKeyStore> logger, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        //public async Task<string> GetServiceFromKey(string apiKey)
        //{
        //    await Init();

        //    return _keys.FirstOrDefault(a => a.Key == apiKey)?.Name;
        //}

        public async Task<(bool, string)> Validate(string? apiKey)
        {
            await Init();

            var ent = _keys.FirstOrDefault(ak => ak.Key == apiKey);

            return ent == null ? _configuration.GetValue<bool>("DevelopKey") ? (true, "Develop") : (false, string.Empty) : (true, ent.Name);
        }

        public async Task<string?> Register(string name)
        {
            using (await _asyncLock.LockAsync())
            {
                _logger.LogInformation($"Generate New Api-Key for {name}");

                await Init();

                if (_keys.Any(ak => ak.Name == name)) return null;

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    await using var context = scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>();
                    var key = Convert.ToBase64String(_hashAlgorithm.Value.ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now + name)));
                    var ent = new ApiKey
                    {
                        Key = key,
                        Name = name
                    };


                    context.ApiKeys.Add(ent);

                    await context.SaveChangesAsync();

                    _keys.Add(ent);
                    return key;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error Generating Hash for {name}");

                    throw;
                }
            }
        }

        public async Task<bool> Remove(string serviceName)
        {
            using (await _asyncLock.LockAsync())
            {
                await Init();

                var ent = _keys.FirstOrDefault(k => k.Name == serviceName);

                if (ent == null) return false;
                _keys.Remove(ent);

                using var scope = _serviceScopeFactory.CreateScope();
                await using var context = scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>();

                ent = await context.ApiKeys.FirstOrDefaultAsync(k => k.Name == serviceName);
                if (ent == null) return false;

                context.Remove(ent);
                await context.SaveChangesAsync();

                return true;
            }
        }

        private async Task Init()
        {
            if (_isInit) return;

            using (await _asyncLock.LockAsync())
            {
                if (_isInit) return;

                _logger.LogInformation("Init Api Key Storage");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    await using var context = scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>();
                    _keys.AddRange(context.ApiKeys.AsNoTracking());
                }

                _isInit = true;
            }
        }

        public void AddTemporary(string key)
        {
            _keys.Add(new ApiKey
            {
                Key = key,
                Name = key
            });
        }
    }
}