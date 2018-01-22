using System;
using System.Collections.Generic;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.Data
{
    [ExportRepositoryExtender]
    public sealed class MainDatabase : CommonRepositoryExtender<DatabaseImpl>
    {
        public override IEnumerable<Type> GetRepositoryTypes()
        {
            yield return typeof(Repository<JobEntity, string>);
            yield return typeof(Repository<JobRunEntity, int>);
            yield return typeof(Repository<SetupEntity, int>);
        }
    }
}