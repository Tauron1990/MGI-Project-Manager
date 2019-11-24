using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using Scrutor;

namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    [ServiceDescriptor(typeof(RepositoryManager))]
    public class RepositoryManager
    {
        private readonly GitHubClient _client;

        public RepositoryManager(GitHubClient client) => _client = client;

        public async Task<Repository> GetRepository(string name)
        {
            var arr = name.Split('/');
            return await _client.Repository.Get(arr[0], arr[1]);
        }

        public async Task<IEnumerable<Branch>> GetBranchs(Repository repository) 
            => await _client.Repository.Branch.GetAll(repository.Id);
    }
}