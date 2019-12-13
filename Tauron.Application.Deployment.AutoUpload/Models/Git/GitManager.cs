using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;

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

        public void SyncBranch(string repository, string branch, string path, ProgressHandler progressHandler, TransferProgressHandler transferProgressHandler)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            Repository.Clone(repository, path, new CloneOptions {BranchName = branch, OnProgress = progressHandler, OnTransferProgress = transferProgressHandler});
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

        public void CommitRepo(RegistratedRepository repository)
        {
            if (!Repository.IsValid(repository.RealPath)) return;

            using var repo = new Repository(repository.RealPath);

            StageChanges(repo);
        }

        private static void StageChanges(IRepository repo)
        {
            try
            {

                var status = repo.RetrieveStatus();
                var filePaths = status.Modified.Select(mods => mods.FilePath).ToList();
                Commands.Stage(repo, filePaths);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:RepoActions:StageChanges " + ex.Message);
            }
        }

        private void CommitChanges(IRepository repo)
        {

            repo.Commit("updating files..", new Signature(_settings.UserName, _settings.EMailAdress, DateTimeOffset.Now),
                new Signature(_settings.UserName, _settings.EMailAdress, DateTimeOffset.Now));
        }

        private void PushChanges(IRepository repo, string branch)
        {
            var remote = repo.Network.Remotes["origin"];
            var pushRefSpec = @"refs/heads/" + branch;

            repo.Network.Push(remote, pushRefSpec);
        }
    }
}