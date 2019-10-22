using System;
using CQRSlite.Commands;

namespace Tauron.MgiManager.User.Shared.Command
{
    public class RemoveClaimFromRoleCommand : ICommand
    {
        public Guid Role { get; set; }

        public string Data { get; set; }

        public RemoveClaimFromRoleCommand(Guid role, string data)
        {
            Role = role;
            Data = data;
        }

        public RemoveClaimFromRoleCommand()
        {
            
        }
    }
}