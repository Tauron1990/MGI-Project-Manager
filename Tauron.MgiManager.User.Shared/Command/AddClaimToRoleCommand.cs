using System;
using CQRSlite.Commands;

namespace Tauron.MgiManager.User.Shared.Command
{
    public sealed class AddClaimToRoleCommand : ICommand
    {
        public Guid Role { get; set; }

        public string Data { get; set; }
 
        public Guid ClaimId { get; set; }

        public AddClaimToRoleCommand()
        {
            
        }

        public AddClaimToRoleCommand(Guid role, string data, Guid claimId)
        {
            Role = role;
            Data = data;
            ClaimId = claimId;
        }
    }
}