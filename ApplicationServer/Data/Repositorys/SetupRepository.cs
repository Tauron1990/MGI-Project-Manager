using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys
{
    public sealed class SetupRepository : Repository<SetupEntity, int>
    {
        public SetupRepository(IDatabase database) : base(database)
        {
        }
    }
}