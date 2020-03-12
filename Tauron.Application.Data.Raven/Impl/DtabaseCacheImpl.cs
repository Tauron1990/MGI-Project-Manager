using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Options;

namespace Tauron.Application.Data.Raven.Impl
{
    public sealed class DatabaseCacheImpl : IDatabaseCache, IDisposable
    {
        private readonly ReaderWriterLockSlim _changeLock = new ReaderWriterLockSlim(0);
        private readonly IOptionsMonitor<DatabaseOption> _options;
        private readonly ConcurrentDictionary<string, DatabaseRootImpl> _databases = new ConcurrentDictionary<string, DatabaseRootImpl>();
        private readonly IDisposable _changeToken;

        public DatabaseCacheImpl(IOptionsMonitor<DatabaseOption> options)
        {
            _options = options;
            _changeToken = _options.OnChange((option, s) =>
                                             {
                                                 _changeLock.EnterWriteLock();
                                                 try
                                                 {
                                                     foreach (var impl in _databases.Values) 
                                                         impl.OptionsChanged(option);
                                                 }
                                                 finally
                                                 {
                                                     _changeLock.ExitWriteLock();
                                                 }
                                             });
        }

        public IDatabaseRoot Get(string databaseName) 
            => _databases.GetOrAdd(databaseName, s => new DatabaseRootImpl(_options.CurrentValue, _changeLock, databaseName).Initislize());

        public void Dispose()
        {
            foreach (var root in _databases.Values) 
                root.Dispose();

            _databases.Clear();
            _changeToken.Dispose();
            _changeLock.Dispose();
        }
    }
}