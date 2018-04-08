using System;
using System.Collections.Generic;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data
{
    [ExportRepositoryExtender]
    public sealed class MainDatabase : CommonRepositoryExtender<DatabaseImpl>
    {
        public override IEnumerable<(Type, Type)> GetRepositoryTypes()
        {
            yield break;
        }
    }
}