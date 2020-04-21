using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Serilog;
using Microsoft.Extensions.Options;
using Neleus.DependencyInjection.Extensions;
using Raven.Client.Documents;
using Serilog.Context;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Engine.Data;
using Tauron.Application.Logging;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public class RepositoryManager : IRepositoryManager, IDisposable
    {
        private const string RepoContextProperty = "TargetRepository";

        private readonly IDisposable _subscription;


        private readonly IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> _factory;
        private readonly ISLogger<RepositoryManager> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IPushMessager _messager;
        private readonly IRepoFactory _repoFactory;
        private IDatabaseRoot _database;

        public RepositoryProvider[] Providers { get; }

        public RepositoryManager(IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> factory, ISLogger<RepositoryManager> logger, IDatabaseCache database, 
            IOptionsMonitor<LocalSettings> settings, IFileSystem fileSystem, IPushMessager messager, IRepoFactory repoFactory)
        {
            _factory = factory;
            _logger = logger;
            _fileSystem = fileSystem;
            _messager = messager;
            _repoFactory = repoFactory;

            Providers = factory.GetMetadata().ToArray();
            _database = database.Get(settings.CurrentValue.DatabaseName);

            _subscription = settings.OnChange(ls => Interlocked.Exchange(ref _database, database.Get(ls.DatabaseName)));
        }

        public async Task<(string? msg, bool ok)> Register(string name, string provider, string source, string comment)
        {
            using (LogContext.PushProperty(RepoContextProperty, name))
            {
                using var session = _database.OpenSession(false);

                if (await session.Query<RegistratedRepositoryEntity>().AnyAsync(rr => rr.Name == name))
                    return ("Repository Existiert schon.", false);
                if (Providers.FirstOrDefault(p => p.Id == provider) == null)
                    return ("Provider nicht gefunden.", false);

                _logger.Information("Creating Software Repository Registration: {Name}", name);

                var data = new RegistratedRepositoryEntity
                           {
                               Name = name,
                               Provider = provider,
                               Source = source,
                               Comment = comment,
                               TargetPath = name
                           };

                var dic = _fileSystem.RepositoryRoot.GetDirectory(data.TargetPath);
                if (dic.Exist)
                    dic.Delete();

                await _factory.GetByName(provider).Init(data, dic);

                await session.StoreAsync(data);
                await session.SaveChangesAsync();

                return (null, await SyncRepo(data));
            }
        }

        public async Task<(string? msg, bool ok)> Delete(string name)
        {
            using (LogContext.PushProperty(RepoContextProperty, name))
            {
                try
                {
                    using var session = _database.OpenSession(false);
                    var data = await session.Query<RegistratedRepositoryEntity>().FirstOrDefaultAsync(rr => rr.Name == name);
                    if (data == null) return ("Kein Repository Gefunden", false);

                    var provider = _factory.GetByName(data.Provider);
                    session.Delete(data.Id ?? throw new InvalidOperationException("No Id for Repository Found"));
                    await provider.Delete(data, _fileSystem.RepositoryRoot.GetDirectory(data.TargetPath));

                    await session.SaveChangesAsync();

                    return (null, true);
                }
                catch (Exception e)
                {
                    LogTo.Error(e, "Error on Delete Repository");
                    return ($"Error: {e.Message}", false);
                }
            }
        }

        public Task<RegistratedReporitory[]> GetAllRepositorys()
        {
            using var session = _database.OpenSession();

            return Task.FromResult(
                session.Query<RegistratedRepositoryEntity>()
                   .Select(rr => new RegistratedReporitory(rr, Providers.First(p => p.Id == rr.Provider)))
                   .ToArray());

        }

        public async Task<(SoftwareRepository? repo, string msg)> Get(string name)
        {
            using (LogContext.PushProperty(RepoContextProperty, name))
            {
                using var session = _database.OpenSession();

                var result = await session.Query<RegistratedRepositoryEntity>().FirstOrDefaultAsync(rr => rr.Name == name);

                if (result == null)
                    return (null, "Repository nicht gefunden.");
                if (!result.SyncCompled)
                    return (null, "Repository nicht Syncronisiert.");

                return (await _repoFactory.Read(_fileSystem.RepositoryRoot.GetDirectory(result.TargetPath)), string.Empty);
            }
        }

        public async Task SyncAll()
        {
            using (LogContext.PushProperty("Procedure", "SyncAll"))
            {
                using var session = _database.OpenSession();

                foreach (var reporitory in session.Query<RegistratedRepositoryEntity>().Where(rr => rr.SyncCompled))
                {
                    try
                    {
                        await SyncRepoImpl(reporitory);
                    }
                    catch (Exception e)
                    {
                        LogTo.Error(e, "Error On Sync Repository");
                        await _messager.SyncError(reporitory.Name, e.Message);
                    }
                }
            }
        }

        public void Dispose() 
            => _subscription.Dispose();

        private async Task<bool> SyncRepo(RegistratedRepositoryEntity repositoryEntity)
        {
            using (LogContext.PushProperty(RepoContextProperty, repositoryEntity.Name))
            {
                try
                {
                    await SyncRepoImpl(repositoryEntity);

                    using var session = _database.OpenSession(false);

                    var name = repositoryEntity.Name;
                    repositoryEntity = await session.Query<RegistratedRepositoryEntity>().FirstAsync(e => e.Name == name);
                    repositoryEntity.SyncCompled = true;

                    await session.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error while Background Syncronize Repository: {Name}", repositoryEntity.Name);
                    await Delete(repositoryEntity.Name);

                    await _messager.SyncError(repositoryEntity.Name, e.Message);
                    return false;
                }

            }
        }

        private async Task SyncRepoImpl(RegistratedRepositoryEntity repositoryEntity)
        {
            using (LogContext.PushProperty(RepoContextProperty, repositoryEntity.Name))
            {
                var provider = _factory.GetByName(repositoryEntity.Provider);
                await provider.Sync(repositoryEntity, _fileSystem.RepositoryRoot.GetDirectory(repositoryEntity.TargetPath));
            }
        }
    }
}