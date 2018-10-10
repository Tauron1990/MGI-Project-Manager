using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer.Data
{
    [PublicAPI]
    public interface IDatabaseAcess : IDisposable
    {
        void SaveChanges();

        T GetRepository<T>()
            where T : class;

        T GetContext<T>();

        Task SaveChangesAsync(CancellationToken sourceToken);
    }

    [Export(typeof(RepositoryFactory))]
    [PublicAPI]
    public class RepositoryFactory : INotifyBuildCompled
    {
        private class NullDispose : IDatabaseAcess
        {
            private readonly RepositoryFactory _fac;

            public NullDispose(RepositoryFactory fac)
            {
                _fac = fac;
            }

            public void Dispose()
            {
            }

            public void SaveChanges()
            {
            }

            public T GetRepository<T>() where T : class => _fac.GetRepository<T>();
            public T GetContext<T>()
            {
                return default(T);
            }

            public Task SaveChangesAsync(CancellationToken sourceToken) => Task.CompletedTask;
        }

        private class DatabaseDisposer : IDatabaseAcess
        {
            private readonly GroupDictionary<IDatabaseIdentifer, object> _databases;
            private readonly Action                                      _exitAction;
            private readonly RepositoryFactory _fac;

            public DatabaseDisposer(GroupDictionary<IDatabaseIdentifer, object> databases, Action exitAction, RepositoryFactory fac)
            {
                _databases  = databases;
                _exitAction = exitAction;
                _fac = fac;
            }

            public void Dispose()
            {
                foreach (var database in _databases.Keys)
                    database.Dispose();

                _exitAction();
            }

            public void SaveChanges()
            {
                foreach (var database in _databases.Keys.OfType<IDatabase>())
                    database.SaveChanges();
            }

            public T GetRepository<T>() where T : class => _fac.GetRepository<T>();

            public T GetContext<T>() => (T) GetDbContext(typeof(T));

            private object GetDbContext(Type dbContext) => _databases.Keys.SingleOrDefault(di => di.Context?.GetType() == dbContext)?.Context;

            public Task SaveChangesAsync(CancellationToken sourceToken) => Task.WhenAll(_databases.Keys.OfType<IDatabase>().Select(d => d.SaveChangesAsync(sourceToken)));
        }

        private static RepositoryFactory _repositoryFactory;

        public static RepositoryFactory Factory => _repositoryFactory ?? (_repositoryFactory = CommonApplication.Current.Container.Resolve<RepositoryFactory>());

        private object _sync = new object();
        private bool             _compositeMode;
        private DatabaseDisposer _databaseDisposer;

        private Dictionary<Type, (IDatabaseFactory, Type)> _databaseFactories;

        [Inject]
        private List<IRepositoryExtender> _extenders;

        private GroupDictionary<IDatabaseIdentifer, object> _repositorys;

        void INotifyBuildCompled.BuildCompled()
        {
            _databaseFactories = new Dictionary<Type, (IDatabaseFactory, Type)>();

            foreach (var repositoryExtender in _extenders)
            {
                var fac = repositoryExtender.DatabaseFactory;

                foreach (var repositoryType in repositoryExtender.GetRepositoryTypes())
                {
                    _databaseFactories.Add(repositoryType.Item1, (fac, repositoryType.Item2));
                }
            }

            _extenders = null;
        }

        public IDatabaseAcess EnterCompositeMode()
        {
            var enter = Enter();
            _compositeMode = true;
            return enter;
        }

        public IDatabaseAcess Enter()
        {
            if (_compositeMode) return new NullDispose(this);
            bool locked = false;

            try
            {
                if (!Monitor.TryEnter(_sync, TimeSpan.FromMinutes(1))) throw new InvalidOperationException("Only One Database Acess Alowed");
                locked = true;

                _repositorys      = new GroupDictionary<IDatabaseIdentifer, object>();
                _databaseDisposer = new DatabaseDisposer(_repositorys, Exit, this);

                return _databaseDisposer;
            }
            finally
            {
                if(locked)
                    Monitor.Exit(_sync);
            }
        }

        public TRepo GetRepository<TRepo>()
            where TRepo : class
        {
            return (TRepo) GetRepository(typeof(TRepo));
        }

        public object GetRepository(Type repoType)
        {
            if (!_databaseFactories.TryGetValue(repoType, out var fac)) throw new InvalidOperationException("No Repository Registrated");

            var dbEnt = _repositorys.FirstOrDefault(p => p.Key.Id == fac.Item1.Id);

            var repo = dbEnt.Value?.FirstOrDefault(obj => obj.GetType() == repoType);

            if (repo != null) return repo;

            var db = dbEnt.Key ?? fac.Item1.CreateDatabase();


            var robj = Activator.CreateInstance(fac.Item2, db);
            if (dbEnt.Key == null)
                _repositorys.Add(db, robj);

            return robj;
        }

        private void Exit()
        {
            _repositorys      = null;
            _databaseDisposer = null;
            _compositeMode    = false;
        }
    }
}