using System;
using System.Collections.Generic;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data
{
    [ExportRepositoryExtender]
    public sealed class MainDatabase : CommonRepositoryExtender<DatabaseImpl>
    {
        public override IEnumerable<(Type, Type)> GetRepositoryTypes()
        {
            yield return (typeof(IJobRepository), typeof(JobRepository));
            yield return (typeof(IJobRunRepository), typeof(JobRunRepository));
            yield return (typeof(ISetupRepository), typeof(SetupRepository));
            yield return (typeof(IUserRepsoitory), typeof(UserRepository));
        }
    }
}