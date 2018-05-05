using System;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Generic.Extensions
{
    public interface IUserServiceExtension
    {
        event Action<GenericServiceResult> PasswordChanged;
    }
}