using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server.Core.Impl
{
    public class ApiKeyStore : IApiKeyStore
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ApiKeyStore> _logger;

        private readonly Lazy<HashAlgorithm> _hashAlgorithm =
            new Lazy<HashAlgorithm>(() => HashAlgorithm.Create("sha256"));

        private List<ApiKey> _keys;

        public ApiKeyStore(IServiceScopeFactory serviceScopeFactory, ILogger<ApiKeyStore> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task<string> GetServiceFromKey(string apiKey)
        {
            await Init();

            return _keys.FirstOrDefault(a => a.Key == apiKey)?.Name;
        }

        public async Task<bool> Validate(string apiKey)
        {
            await Init();

            return _keys.Any(ak => ak.Key == apiKey);
        }

        public async Task<string> Register(string name)
        {
            _logger.LogInformation($"Generate New Api-Key for {name}");

            await Init();

            if (_keys.Any(ak => ak.Name == name)) return null;

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>())
                    {

                        string key =
                            Convert.ToBase64String(
                                _hashAlgorithm.Value.ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now + name)));

                        context.ApiKeys.Add(new ApiKey
                                            {
                                                Key = key,
                                                Name = name
                                            });

                        await context.SaveChangesAsync();

                        return key;
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Error Generating Hash for {name}");

                throw;
            }
        }

        private Task Init()
        {
            if (_keys != null) return Task.CompletedTask;

            _logger.LogInformation("Init Api Key Storage");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>())
                    _keys = context.ApiKeys.AsNoTracking().ToList();
            }

            return Task.CompletedTask;
        }
    }
}