using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Actions;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.BL.Tasks.Operations
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    [Export(typeof(IOperationAction))]
    public sealed class MultifileOperation : IOperationAction
    {
        private readonly IFilesHub _filesHub;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobNameMatcher _jobNameMatcher;
        private readonly ILogger<MultifileOperation> _logger;

        public MultifileOperation(IEventDispatcher eventDispatcher, IUnitOfWork repository, IJobNameMatcher jobNameMatcher, ILogger<MultifileOperation> logger)
        {
            _filesHub = eventDispatcher.GetToken<IFilesHub>().Hub;
            _unitOfWork = repository;
            _jobNameMatcher = jobNameMatcher;
            _logger = logger;
        }

        public string Name => OperationNames.MultiFileOperation;

        public async Task<OperationSetup[]> Execute(Operation op)
        {
            List<OperationSetup> newOperations = new List<OperationSetup>();
            string user = op.OperationContext[OperationMeta.MultiFile.UserName];

            foreach (var file in op.OperationContext)
            {
                if(file.Key == "UserName") continue;

                var result = _jobNameMatcher.GetMatch(file.Key);
                if (result.Success)
                {
                    string name = result.Value;
                    name = _jobNameMatcher.EditJobName(name);

                    await _unitOfWork.FileRepository.AddFile(new FileEntity
                    {
                        Age = DateTime.Now,
                        Name = name,
                        Path = file.Value,
                        User = user
                    });
                }
                else
                {
                    newOperations.Add(new OperationSetup(
                        OperationNames.LinkingFileOperation, 
                        OperationNames.FileOperationType,
                        new Dictionary<string, string>
                        {
                            { OperationMeta.Linker.FilePath, file.Value },
                            { OperationMeta.Linker.FileName, file.Key },
                            { OperationMeta.Linker.UserName, user },
                            { OperationMeta.Linker.StartId, op.OperationId }
                        }, 
                        DateTime.Now + TimeSpan.FromDays(3)));
                }
            }

            await _unitOfWork.SaveChanges();

            return await newOperations.ToAsyncEnumerable().ToArray();
        }

        public async Task PostExecute(Operation op) 
            => await _filesHub.SendMultifileProcessingCompled(op.OperationId, false, String.Empty);

        public Task<bool> Remove(Operation op)
        {
            try
            {
                foreach (var file in op.OperationContext.Select(p => p.Value))
                {
                    var info = new FileInfo(file);
                    if(info.Exists)
                        info.Delete();
                }

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error on Delete files: {op.OperationId}");
                return Task.FromResult(false);
            }
        }

        public async Task Error(Operation op, Exception e)
        {
            await _filesHub.SendMultifileProcessingCompled(op.OperationId, true, e.Message);
            await Remove(op);
        }
    }
}