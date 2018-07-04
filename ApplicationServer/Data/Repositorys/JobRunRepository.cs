using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys
{
    public sealed class JobRunRepository : Repository<JobRunEntity, int>, IJobRunRepository
    {
        public JobRunRepository(IDatabase database) : base(database)
        {
        }
    }
}