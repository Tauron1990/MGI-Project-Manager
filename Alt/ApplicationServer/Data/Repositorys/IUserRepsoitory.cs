using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys
{
    public interface IUserRepsoitory : IRepository<UserEntity, string>
    {
        
    }

    public class UserRepository : Repository<UserEntity, string>, IUserRepsoitory
    {
        public UserRepository(IDatabase database) : base(database)
        {
        }
    }
}