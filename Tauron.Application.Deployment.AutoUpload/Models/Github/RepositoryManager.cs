using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Anotar.Serilog;
using Octokit;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.ToolUI.Login;
using FileMode = System.IO.FileMode;

namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    [ServiceDescriptor(typeof(RepositoryManager))]
    public class RepositoryManager
    {
        private readonly GitHubClient _client;

        private readonly Dictionary<string, InternalStore> _gitHubCredinals = new Dictionary<string, InternalStore>();
        private readonly InputService _inputService;

        public RepositoryManager(GitHubClient client, InputService inputService)
        {
            _client = client;
            _inputService = inputService;
        }

        public async Task<Repository> GetRepository(string name)
        {
            var arr = name.Split('/');
            return await _client.Repository.Get(arr[0], arr[1]);
        }

        public async Task<Repository> CreateRepository(string name)
        {
            var arr = name.Split('/');

            return await ExceuteAut(arr[0], async client => await _client.Repository.Create(new NewRepository(arr[1])));
        }

        public async Task<IEnumerable<Branch>> GetBranchs(Repository repository)
            => await _client.Repository.Branch.GetAll(repository.Id);

        public async Task<(string, int)> UploadAsset(long repoId, string fileName, string assetName, string name)
        {
            return await ExceuteAut(name, async client =>
                                          {
                                              LogTo.Information("Create Release: {Name}", assetName);
                                              var release = await client.Repository.Release.Create(repoId, new NewRelease(assetName) {Body = $"Automated Release of {assetName}"});
                                              await using var rawData = File.Open(fileName, FileMode.Open);
                                              LogTo.Information("Upload Asset: {Name}", assetName);
                                              var asset = await client.Repository.Release.UploadAsset(release, new ReleaseAssetUpload(assetName, "application/zip", rawData, null));

                                              return (asset.BrowserDownloadUrl, release.Id);
                                          });
        }

        public async Task DeleteRelease(long repo, int release, string name)
        {
            await ExceuteAut(name, async c =>
                                   {
                                       LogTo.Information("Delete Release: {Repo}--{Release}", repo, release);
                                       await c.Repository.Release.Delete(repo, release);
                                       return string.Empty;
                                   });
        }

        private async Task<TType> ExceuteAut<TType>(string name, Func<GitHubClient, Task<TType>> exec)
        {
            LogTo.Information("Get Credinals for: {Name}", name);
            if (!_gitHubCredinals.TryGetValue(name, out var credinals))
            {
                credinals = new InternalStore(name, _inputService);
                _gitHubCredinals[name] = credinals;
            }

            try
            {

                _client.Credentials = new Credentials(await credinals.GetCredentials());
                LogTo.Information("Execute Request");
                return await exec(_client);
            }
            catch (ApiException e)
            {
                if (e.ApiError?.Message == "Bad credentials")
                    credinals.Invalidate();
                throw;
            }
            finally
            {
                _client.Credentials = Credentials.Anonymous;
            }
        }

        private class InternalStore
        {
            private readonly string _name;
            private readonly InputService _service;

            public InternalStore(string name, InputService service)
            {
                _name = name;
                _service = service;
            }

            public Task<string?> GetCredentials()
                => Task.FromResult(SecureStringToString(_service.GetGitHubToken(_name)));

            public void Invalidate()
                => _service.DeleteCredinals(_name);

            private string? SecureStringToString(SecureString? value)
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
    }
}