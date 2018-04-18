using System.ServiceModel;
using System.ServiceModel.Channels;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public sealed class AdminClient : ClientHelperBase<IAdminService>, IAdminService
    {
        public AdminClient(Binding binding, EndpointAddress adress)
            : base(binding, adress)
        {
        }

        public void AdminLogin(string password)
        {
            Secure(() => Channel.AdminLogin(password));
        }

        public bool IsAdminPasswordNull()
        {
            return Secure(() => Channel.IsAdminPasswordNull());
        }

        public GenericServiceResult CreateUser(string userName, string password)
        {
            return Secure(() => Channel.CreateUser(userName, password));
        }

        public GenericServiceResult DeleteUser(string userName)
        {
            return Secure(() => Channel.DeleteUser(userName));
        }

        public void AdminLogout()
        {
            Secure(() => Channel.AdminLogout());
        }
    }
}