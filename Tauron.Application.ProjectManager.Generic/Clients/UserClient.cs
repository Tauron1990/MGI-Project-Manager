using System.ServiceModel;
using System.ServiceModel.Channels;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public sealed class UserClient : ClientHelperBase<IUserService>, IUserService
    {
        public UserClient(Binding binding, EndpointAddress adress) : base(binding, adress)
        {
        }

        public string[] GetUsers()
        {
            return Secure(() => Channel.GetUsers());
        }

        public GenericServiceResult ChangePassword(string name, string newPassword, string oldPassword)
        {
            return Secure(() => Channel.ChangePassword(name, newPassword, oldPassword));
        }
    }
}