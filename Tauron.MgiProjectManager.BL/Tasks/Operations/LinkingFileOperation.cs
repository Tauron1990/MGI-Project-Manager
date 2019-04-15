using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Actions;
using Tauron.MgiProjectManager.Dispatcher.Model;
using Tauron.MgiProjectManager.Resources;

namespace Tauron.MgiProjectManager.BL.Tasks.Operations
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    [Export(typeof(IOperationAction))]
    public class LinkingFileOperation : IOperationAction
    {
        private readonly IJobNameMatcher _matcher;
        private readonly IFilesHub _filesHub;
        private readonly ILogger<LinkingFileOperation> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public string Name => OperationNames.LinkingFileOperation;

        public LinkingFileOperation(IJobNameMatcher matcher, IEventDispatcher eventDispatcher, ILogger<LinkingFileOperation> logger, IUnitOfWork repository)
        {
            _matcher = matcher;
            _filesHub = eventDispatcher.GetToken<IFilesHub>().Hub;
            _logger = logger;
            _unitOfWork = repository;
        }

        public async Task<OperationSetup[]> Execute(Operation op)
        {
            
            string name = op.OperationContext[OperationMeta.Linker.RequestedName];
            var match = _matcher.GetMatch(name);
            if (match.Success)
            {
                name = _matcher.EditJobName(match.Value);
                await _unitOfWork.FileRepository.AddFile(new FileEntity
                {
                    Age = DateTime.Now,
                    Name = name,
                    Path = op.OperationContext[OperationMeta.Linker.FilePath],
                    User = op.OperationContext[OperationMeta.Linker.UserName]
                });
                await _filesHub.SendLinkingCompled(op.OperationId, true, string.Empty);
                await _unitOfWork.SaveChanges();

                return Array.Empty<OperationSetup>();
            }

            await _filesHub.SendLinkingCompled(op.OperationId, false, BLRes.LinkingFileOperation_Incompatible_JobName);
            return Array.Empty<OperationSetup>();
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
            => await _filesHub.SendLinkingCompled(op.OperationId, false, e.Message);
    }
}