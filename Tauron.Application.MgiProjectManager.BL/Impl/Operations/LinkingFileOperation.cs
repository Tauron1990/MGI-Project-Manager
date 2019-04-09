using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.BL.Contracts.Helper;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.Application.MgiProjectManager.BL.Impl.Hubs;

namespace Tauron.Application.MgiProjectManager.BL.Impl.Operations
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class LinkingFileOperation : IOperationAction
    {
        private readonly IJobNameMatcher _matcher;
        private readonly IHubContext<FilesHub, IFilesHub> _filesHub;

        public string Name => OperationNames.LinkingFileOperation;

        public LinkingFileOperation(IJobNameMatcher matcher, IHubContext<FilesHub, IFilesHub> filesHub)
        {
            _matcher = matcher;
            _filesHub = filesHub;
        }

        public Task<Operation[]> Execute(Operation op)
        {
            
        }

        public Task PostExecute(Operation op)
        {
        }

        public Task<bool> Remove(Operation op)
        {
        }

        public Task Error(Operation op, Exception e)
        {
        }
    }
}