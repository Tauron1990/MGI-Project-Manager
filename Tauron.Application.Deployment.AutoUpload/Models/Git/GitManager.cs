using System;
using System.IO;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;

namespace Tauron.Application.Deployment.AutoUpload.Models.Git
{
    [ServiceDescriptor(typeof(GitManager))]
    public class GitManager
    {
        private readonly Settings _settings;

        public GitManager(Settings settings)
        {
            _settings = settings;
        }

        public string SyncBranch(string repository, string branch, string path, ProgressHandler progressHandler, TransferProgressHandler transferProgressHandler)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Repository.Clone(repository, path, new CloneOptions {BranchName = branch, OnProgress = progressHandler, OnTransferProgress = transferProgressHandler});
        }

        public MergeResult SyncRepo(string path, ProgressHandler progressHandler, TransferProgressHandler transferProgressHandler)
        {
            using var repo = new Repository(path);
            return Commands.Pull(repo,
                                 new Signature(_settings.UserName, _settings.EMailAdress, _settings.UserWhen),
                                 new PullOptions
                                 {
                                     FetchOptions = new FetchOptions
                                                    {
                                                        OnProgress = progressHandler,
                                                        OnTransferProgress = transferProgressHandler
                                                    },
                                     MergeOptions = new MergeOptions
                                                    {
                                                        CommitOnSuccess = true,
                                                        FailOnConflict = true
                                                    }
                                 });
        }
    }
}