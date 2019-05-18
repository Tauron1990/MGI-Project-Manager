using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server.Core.Impl
{
    public class ApiKeyStore : IApiKeyStore
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly Lazy<HashAlgorithm> _hashAlgorithm =
            new Lazy<HashAlgorithm>(() => HashAlgorithm.Create("sha256"));

        private List<ApiKey> _keys;

        public ApiKeyStore(IServiceScopeFactory serviceScopeFactory)
            => _serviceScopeFactory = serviceScopeFactory;

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
            await Init();

            if (_keys.Any(ak => ak.Name == name)) return null;

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

        private Task Init()
        {
            if (_keys != null) return Task.CompletedTask;

            using (var scope = _serviceScopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>())
                _keys = context.ApiKeys.ToList();

            return Task.CompletedTask;
        }
    }
}