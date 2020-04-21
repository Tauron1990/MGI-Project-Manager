using System;
using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using LibGit2Sharp;
using Microsoft.Extensions.Options;
using Tauron.Application.Deployment.Server.Engine.Data;
using Tauron.Application.Files.VirtualFiles;
using Tauron.Application.Files.VirtualFiles.LocalFileSystem;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public sealed class GitProvider : IRepoProvider
    {
        private readonly IOptionsMonitor<LocalSettings> _settings;

        public GitProvider(IOptionsMonitor<LocalSettings> settings) 
            => _settings = settings;

        public Task Delete(RegistratedRepositoryEntity repository, IDirectory directory)
        {
            return Task.Run(() =>
            {
                LogTo.Information("Delete Repository Folder {RepoPath}", repository.TargetPath);
                directory.Delete();
            });
        }

        public Task Init(RegistratedRepositoryEntity repository, IDirectory directory) 
            => Task.Run(() =>
            {
                if (directory is LocalDirectory)
                {
                    LogTo.Information("Clone Repository {RepoPath} -- {Url}", repository.TargetPath, repository.Source);
                    Repository.Clone(repository.Source, repository.TargetPath);
                }
                else
                {
                    LogTo.Error("Git provider only supports LocalDirectory");
                    throw new NotSupportedException();
                }
            });

        public Task Sync(RegistratedRepositoryEntity repository, IDirectory directory) 
            => Task.Run(() =>
            {
                if (directory is LocalDirectory)
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
                }
                else
                {
                    LogTo.Error("Git provider only supports LocalDirectory");
                    throw new NotSupportedException();
                }
            });
    }
}