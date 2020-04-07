﻿using System;
using System.Threading;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;

namespace Tauron.Application.Data.Raven.Impl
{
    public sealed class DatabaseRootImpl : IDatabaseRoot, IDisposable
    {
        private static readonly DocumentConventions DocumentConventions = new DocumentConventions();

        private readonly ReaderWriterLockSlim _changeLock;
        private readonly string _databaseName;
        private readonly MemoryConfig _memoryConfig;
        private IDocumentStore? _documentStore;
        private InMemoryStore? _inMemoryStore;

        private DatabaseOption? _option;

        public DatabaseRootImpl(DatabaseOption option, ReaderWriterLockSlim changeLock, string databaseName, MemoryConfig memoryConfig)
        {
            _option = option;
            _changeLock = changeLock;
            _databaseName = databaseName;
            _memoryConfig = memoryConfig;
        }

        public IDatabaseSession OpenSession(bool noTracking = true)
        {
            SessionBase? session = null;

            if (_documentStore != null)
                session = new RavenSession(_documentStore, noTracking, _changeLock);
            if (_inMemoryStore != null)
                session = new InMemeorySession(_inMemoryStore, _changeLock);

            session?.Enter();

            return session ?? throw new InvalidOperationException("Database is not Initialized");
        }

        public void Dispose()
        {
        }

        public void OptionsChanged(DatabaseOption options)
        {
            _option = options;
            Initislize();
        }

        public DatabaseRootImpl Initislize()
        {
            _inMemoryStore = null;
            _documentStore?.Dispose();
            _documentStore = null;

            if (_option?.InMemory == true)
            {
                _inMemoryStore = _memoryConfig.MemoryStores.TryGetValue(_databaseName, out var memoryStore) 
                    ? memoryStore 
                    : new InMemoryStore();
            }
            else
                _documentStore = new DocumentStore {Conventions = DocumentConventions, Urls = _option?.Urls, Database = _databaseName}.Initialize();

            return this;
        }
    }
}