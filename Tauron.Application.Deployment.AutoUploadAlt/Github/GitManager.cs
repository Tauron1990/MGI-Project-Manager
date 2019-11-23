using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace Tauron.Application.Deployment.AutoUpload.Github
{
    public static class GitManager
    {
        public static string SyncBranch(string repository, string branch, string tempPath, ProgressHandler progressHandler, TransferProgressHandler transferProgressHandler)
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            return Repository.Clone(repository, tempPath, new CloneOptions {BranchName = branch, IsBare = true, OnProgress = progressHandler, OnTransferProgress = transferProgressHandler});
        }
    }
}