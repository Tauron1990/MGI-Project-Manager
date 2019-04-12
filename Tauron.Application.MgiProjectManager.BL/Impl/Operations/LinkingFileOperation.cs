using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.BL.Contracts.Helper;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.Application.MgiProjectManager.BL.Impl.Hubs;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.BL.Impl.Operations
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class LinkingFileOperation : IOperationAction
    {
        private readonly IJobNameMatcher _matcher;
        private readonly IHubContext<FilesHub, IFilesHub> _filesHub;
        private readonly ILogger<LinkingFileOperation> _logger;
        private readonly IFileRepository _repository;

        public string Name => OperationNames.LinkingFileOperation;

        public LinkingFileOperation(IJobNameMatcher matcher, IHubContext<FilesHub, IFilesHub> filesHub, ILogger<LinkingFileOperation> logger, IFileRepository repository)
        {
            _matcher = matcher;
            _filesHub = filesHub;
            _logger = logger;
            _repository = repository;
        }

        public async Task<Operation[]> Execute(Operation op)
        {
            string name = op.OperationContext[OperationMeta.Linker.RequestedName];
            var match = _matcher.GetMatch(name);
            if (match.Success)
            {
                name = _matcher.EditJobName(match.Value);
                await _repository.AddFile(new FileEntity
                {
                    Age = DateTime.Now,
                    Name = name,
                    Path = op.OperationContext[OperationMeta.Linker.FilePath],
                    User = op.OperationContext[OperationMeta.Linker.UserName]
                });
                await _filesHub.Clients.All.SendLinkingCompled(op.OperationId, true, string.Empty);

                return Array.Empty<Operation>();
            }

            await _filesHub.Clients.All.SendLinkingCompled(op.OperationId, false, WebResources.LinkingFileOperation_Incompatible_JobName);
            return Array.Empty<Operation>();
        }

        public Task PostExecute(Operation op) 
            => Task.CompletedTask;

        public Task<bool> Remove(Operation op)
        {
            try
            {
                string file = op.OperationContext[OperationMeta.Linker.FilePath];
                if (File.Exists(file))
                    File.Delete(file);

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Remove Operation");
                return Task.FromResult(false);
            }
        }

        public async Task Error(Operation op, Exception e) 
            => await _filesHub.Clients.All.SendLinkingCompled(op.OperationId, false, e.Message);
    }
}