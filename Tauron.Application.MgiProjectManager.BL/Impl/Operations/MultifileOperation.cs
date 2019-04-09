using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.BL.Contracts.Helper;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.Application.MgiProjectManager.BL.Impl.Hubs;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.BL.Impl.Operations
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class MultifileOperation : IOperationAction
    {


        private readonly IHubContext<FilesHub, IFilesHub> _filesHub;
        private readonly IFileRepository _repository;
        private readonly IJobNameMatcher _jobNameMatcher;
        private readonly ILogger<MultifileOperation> _logger;

        public MultifileOperation(IHubContext<FilesHub, IFilesHub> filesHub, IFileRepository repository, IJobNameMatcher jobNameMatcher, ILogger<MultifileOperation> logger)
        {
            _filesHub = filesHub;
            _repository = repository;
            _jobNameMatcher = jobNameMatcher;
            _logger = logger;
        }

        public string Name => OperationNames.MultiFileOperation;

        public async Task<Operation[]> Execute(Operation op)
        {
            List<Operation> newOperations = new List<Operation>();
            string user = op.OperationContext[OperationMeta.MultiFile.UserName];

            foreach (var file in op.OperationContext)
            {
                if(file.Key == "UserName") continue;

                var result = _jobNameMatcher.GetMatch(file.Key);
                if (result.Success)
                {
                    string name = result.Value;
                    name = _jobNameMatcher.EditJobName(name);

                    await _repository.AddFile(new FileEntity
                    {
                        Age = DateTime.Now,
                        Name = name,
                        Path = file.Value,
                        User = user
                    });
                }
                else
                {
                    newOperations.Add(new Operation(
                        Guid.NewGuid().ToString("D"), 
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

            return await newOperations.ToAsyncEnumerable().ToArray();
        }

        public async Task PostExecute(Operation op) 
            => await _filesHub.Clients.All.SendMultifileProcessingCompled(op.OperationId, false, String.Empty);

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
            await _filesHub.Clients.All.SendMultifileProcessingCompled(op.OperationId, true, e.Message);
            await Remove(op);
        }
    }
}