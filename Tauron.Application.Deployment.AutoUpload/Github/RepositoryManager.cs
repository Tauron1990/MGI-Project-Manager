using System;
using System.Threading.Tasks;
using Octokit;

namespace Tauron.Application.Deployment.AutoUpload.Github
{
    public static class RepositoryManager
    {
        public static async Task<Repository> GetRepository(string name, GitHubClient client)
        {
            var arr = name.Split('/');
            return await client.Repository.Get(arr[0], arr[1]);
        }

        public static async Task<Branch> GetBranch(Repository repo, Func<Task<string>> nameGetter, GitHubClient client)
        {
            var name = await nameGetter();
            return await client.Repository.Branch.Get(repo.Id, name);
        }
    }
}