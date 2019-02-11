using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public sealed class UserCallBack : CallbackBase<UserClient>, IUserServiceCallback
    {
        public void PasswortChanged(GenericServiceResult result) => Client.OnPasswordChanged(result);
    }
}