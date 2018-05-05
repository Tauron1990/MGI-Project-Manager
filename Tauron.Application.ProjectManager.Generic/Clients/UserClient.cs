using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Tauron.Application.ProjectManager.Generic.Extensions;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public class UserCallBack : IUserServiceCallback
    {
        private UserClient _userClient;

        public void PasswortChanged(GenericServiceResult result) => _userClient.OnPasswordChanged(result);

        public void SetClient(UserClient client) => _userClient = client;
    }

    public sealed class UserClient : ClientHelperBase<IUserService>, IUserService, IUserServiceExtension
    {
        public UserClient(UserCallBack context, Binding binding, EndpointAddress adress) 
            : base(context,  binding, adress) => context.SetClient(this);

        public string[] GetUsers()
        {
            return Secure(() => Channel.GetUsers());
        }

        public void ChangePassword(string name, string newPassword, string oldPassword)
        {
            Secure(() => Channel.ChangePassword(name, newPassword, oldPassword));
        }

        public UserRights GetUserRights(string user)
        {
            return Secure(() => Channel.GetUserRights(user));
        }

        internal void OnPasswordChanged(GenericServiceResult result) => PasswordChanged?.Invoke(result);

        public event Action<GenericServiceResult> PasswordChanged;
    }
}