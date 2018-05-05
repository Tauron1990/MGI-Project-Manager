using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys
{
    public class UserEntity : GenericBaseEntity<string>
    {
        private string _password;
        private string _salt;
        private UserRights _userRights;

        public string Password
        {
            get => _password;
            set => SetWithNotify(ref _password, value);
        }

        public string Salt
        {
            get => _salt;
            set => SetWithNotify(ref _salt, value);
        }

        public UserRights UserRights
        {
            get => _userRights;
            set => SetWithNotify(ref _userRights, value);
        }
    }
}