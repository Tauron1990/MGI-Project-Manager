using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Octokit;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Core;

namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    [ServiceDescriptor(typeof(RepositoryManager))]
    public class RepositoryManager
    {
        private class InternalStore : ICredentialStore
        {
            private readonly string _name;
            private readonly InputService _service;

            public InternalStore(string name, InputService service)
            {
                _name = name;
                _service = service;
            }

            public Task<Credentials> GetCredentials()
            {
                var (userName, passwort) = _service.Request(_name);

                return Task.FromResult(new Credentials(userName, SecureStringToString(passwort)));
            }

            string? SecureStringToString(SecureString? value)
            {
                if (value == null) return null;

                var valuePtr = IntPtr.Zero;
                try
                {
                    valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                    return Marshal.PtrToStringUni(valuePtr);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
                }
            }
        }

        private readonly GitHubClient _client;
        private readonly InputService _inputService;
        private readonly DynamicCredStore _dynamicCredStore;

        public RepositoryManager(GitHubClient client, InputService inputService, DynamicCredStore dynamicCredStore)
        {
            _client = client;
            _inputService = inputService;
            _dynamicCredStore = dynamicCredStore;
        }

        public async Task<Repository> GetRepository(string name)
        {
            var arr = name.Split('/');
            return await _client.Repository.Get(arr[0], arr[1]);
        }

        public async Task<Repository> CreateRepository(string name)
        {
            var arr = name.Split('/');

            return await ExcuteAut(arr[0], async client => await _client.Repository.Create(new NewRepository(arr[1])));
        }

        public async Task<IEnumerable<Branch>> GetBranchs(Repository repository) 
            => await _client.Repository.Branch.GetAll(repository.Id);

        private async Task<TType> ExcuteAut<TType>(string name, Func<GitHubClient, Task<TType>> exec)
        {
            var internalStore = new InternalStore(name, _inputService);
            var old = _dynamicCredStore.CredentialStore;

            try
            {
                _dynamicCredStore.CredentialStore = internalStore;
                return await exec(_client);
            }
            finally
            {
                _dynamicCredStore.CredentialStore = old;
            }
        }
    }
}