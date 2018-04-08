using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public interface IDatabaseAcess : IDisposable
    {
        void SaveChanges();
    }

    [Export(typeof(RepositoryFactory)), PublicAPI]
    public class RepositoryFactory : INotifyBuildCompled
    {
        private class NullDispose : IDatabaseAcess
        {
            public void Dispose()
            {
                
            }

            public void SaveChanges()
            {
                
            }
        }
        private class DatabaseDisposer : IDatabaseAcess
        {
            private readonly GroupDictionary<IDatabaseIdentifer, object> _databases;
            private readonly Action _exitAction;

            public DatabaseDisposer(GroupDictionary<IDatabaseIdentifer, object> databases, Action exitAction)
            {
                _databases = databases;
                _exitAction = exitAction;
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
        }

        [Inject]
        private List<IRepositoryExtender> _extenders;

        private Dictionary<Type, (IDatabaseFactory, Type)> _databaseFactories;
        private GroupDictionary<IDatabaseIdentifer, object> _repositorys;
        private DatabaseDisposer _databaseDisposer;
        private bool _compositeMode;

        public IDatabaseAcess EnterCompositeMode()
        {
            var enter = Enter();
            _compositeMode = true;
            return enter;
        }

        public IDatabaseAcess Enter()
        {
            if(_compositeMode) return new NullDispose();

            if(_databaseDisposer != null) throw new InvalidOperationException("Only One Database Acess Alowed");

            _repositorys = new GroupDictionary<IDatabaseIdentifer, object>();
            _databaseDisposer = new DatabaseDisposer(_repositorys, Exit);

            return _databaseDisposer;
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
            _repositorys = null;
            _databaseDisposer = null;
            _compositeMode = false;
        }

        void INotifyBuildCompled.BuildCompled()
        {
            _databaseFactories = new Dictionary<Type, (IDatabaseFactory, Type)>();

            foreach (var repositoryExtender in _extenders)
            {
                var fac = repositoryExtender.DatabaseFactory;

                foreach (var repositoryType in repositoryExtender.GetRepositoryTypes())
                    _databaseFactories.Add(repositoryType.Item1, (fac, repositoryType.Item2));
            }

            _extenders = null;
        }
    }
}