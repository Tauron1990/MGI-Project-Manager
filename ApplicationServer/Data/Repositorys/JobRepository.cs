using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys
{
    public sealed class JobRepository : Repository<JobEntity, string>, IJobRepository
    {
        public JobRepository(IDatabase database) : base(database)
        {
        }
    }
}