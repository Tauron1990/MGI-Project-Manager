﻿using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using LibGit2Sharp;
using Microsoft.Extensions.Options;
using Tauron.Application.Deployment.Server.Engine.Data;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public sealed class GitProvider : IRepoProvider
    {
        private readonly IOptionsMonitor<LocalSettings> _settings;

        public GitProvider(IOptionsMonitor<LocalSettings> settings) 
            => _settings = settings;

        public Task Delete(RegistratedRepositoryEntity repository)
        {
            return Task.Run(() =>
            {
                LogTo.Information("Delete Repository Folder {RepoPath}", repository.TargetPath);
                File.Delete(repository.TargetPath);
            });
        }

        public Task Init(RegistratedRepositoryEntity repository) 
            => Task.Run(() =>
            {
                LogTo.Information("Clone Repository {RepoPath} -- {Url}", repository.TargetPath, repository.Source);
                Repository.Clone(repository.Source, repository.TargetPath);
            });

        public Task Sync(RegistratedRepositoryEntity repository) 
            => Task.Run(() =>
            {
                LogTo.Information("Sync Git Repository {Name}", repository.Name);

                if (!Repository.IsValid(repository.TargetPath))
                {
                    LogTo.Warning("No Valid Git Repository {Name}", repository.Name);
                    return;
                }

                using var repo = new Repository(repository.TargetPath);
                Commands.Pull(repo, _settings.CurrentValue.Signature.Create(), new PullOptions
                                                                               {
                                                                                   MergeOptions = new MergeOptions
                                                                                                  {
                                                                                                      CommitOnSuccess = true,
                                                                                                      FailOnConflict = true
                                                                                                  }
                                                                               });
            });
    }
}