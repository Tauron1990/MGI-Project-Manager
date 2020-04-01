﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Neleus.DependencyInjection.Extensions;
using Raven.Client.Documents;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.Deployment.Server.Engine.Data;
using Tauron.Application.Logging;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public class RepositoryManager : IRepositoryManager, IDisposable
    {
        private readonly IDisposable _subscription;


        private readonly IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> _factory;
        private readonly ISLogger<RepositoryManager> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IPushMessager _messager;
        private IDatabaseRoot _database;

        public RepositoryProvider[] Providers { get; }

        public RepositoryManager(IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> factory, ISLogger<RepositoryManager> logger, IDatabaseCache database, 
            IOptionsMonitor<LocalSettings> settings, IFileSystem fileSystem, IPushMessager messager)
        {
            _factory = factory;
            _logger = logger;
            _fileSystem = fileSystem;
            _messager = messager;

            Providers = factory.GetMetadata().ToArray();
            _database = database.Get(settings.CurrentValue.DatabaseName);

            _subscription = settings.OnChange(ls => Interlocked.Exchange(ref _database, database.Get(ls.DatabaseName)));
        }

        public async Task<(string? msg, bool ok)> Register(string name, string provider, string source, string comment)
        {
            using var session = _database.OpenSession(false);

            if (await session.Query<RegistratedReporitoryEntity>().AnyAsync(rr => rr.Name == name))
                return ("Repository Existiert schon.", false);
            if (Providers.FirstOrDefault(p => p.Id == provider) == null)
                return ("Provider nicht gefunden.", false);

            _logger.Information("Creating Software Repository Registration: {Name}", name);

            var data = new RegistratedReporitoryEntity
                       {
                           Name = name,
                           Provider = provider,
                           Source = source,
                           Comment = comment,
                           TargetPath = Path.Combine(_fileSystem.RepositoryRoot, name)
                       };

            await _factory.GetByName(provider).Init(data);

            await session.StoreAsync(data);
            await session.SaveChangesAsync();

            // ReSharper disable once MethodHasAsyncOverload
            SyncRepo(data);

            return (null, true);
        }

        public async Task Delete(string name)
        {
            using var session = _database.OpenSession(false);
            var data = await session.Query<RegistratedReporitoryEntity>().FirstOrDefaultAsync(rr => rr.Name == name);
            if( data == null) return;

            var provider = _factory.GetByName(data.Provider);
            session.Delete(data.Id ?? throw new InvalidOperationException("No Id for Repository Found"));
            await provider.Delete(data);

            await session.SaveChangesAsync();
        }

        public Task<RegistratedReporitory[]> GetAllRepositorys()
        {
            using var session = _database.OpenSession();

            return Task.FromResult(
                session.Query<RegistratedReporitoryEntity>()
                   .Select(rr => new RegistratedReporitory(rr, Providers.First(p => p.Id == rr.Provider)))
                   .ToArray());
        }

        public async Task<(SoftwareRepository? repo, string msg)> Get(string name)
        {
            using var session = _database.OpenSession();

            var result = await session.Query<RegistratedReporitoryEntity>().FirstOrDefaultAsync(rr => rr.Name == name);

            if (result == null)
                return (null, "Repository nicht gefunden.");
            if (!result.SyncCompled)
                return (null, "Repository nicht Syncronisiert.");

            return (await SoftwareRepository.Read(result.TargetPath), string.Empty);
        }

        public async Task SyncAll()
        {
            using var session = _database.OpenSession();

            foreach (var reporitory in session.Query<RegistratedReporitoryEntity>().Where(rr => rr.SyncCompled)) 
                await SyncRepoAsync(reporitory);
        }

        public void Dispose() 
            => _subscription.Dispose();

        private void SyncRepo(RegistratedReporitoryEntity reporitoryEntity)
        {
            Task.Run(async () =>
                     {
                         try
                         {
                             await SyncRepoAsync(reporitoryEntity);

                             using var session = _database.OpenSession(false);
                             
                             var name = reporitoryEntity.Name;
                             reporitoryEntity = await session.Query<RegistratedReporitoryEntity>().FirstAsync(e => e.Name == name);
                             reporitoryEntity.SyncCompled = true;

                             await session.SaveChangesAsync();
                         }
                         catch (Exception e)
                         {
                             _logger.Error(e, "Error while Background Syncronize Repository: {Name}", reporitoryEntity.Name);
                             await Delete(reporitoryEntity.Name);

                             await _messager.SyncError(reporitoryEntity.Name, e.Message);
                         }
                     });
        }

        private async Task SyncRepoAsync(RegistratedReporitoryEntity reporitoryEntity)
        {
            var provider = _factory.GetByName(reporitoryEntity.Provider);
            await provider.Sync(reporitoryEntity);
        }
    }
}