using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys
{
    public class UserEntity : GenericBaseEntity<string>
    {
        public string Password { get; set; }

        public string Salt { get; set; }
    }
}