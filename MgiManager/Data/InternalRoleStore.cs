using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using Microsoft.AspNetCore.Identity;
using Tauron.MgiManager.User.Shared.Command;

namespace MgiManager.Data
{
    public sealed class InternalRoleStore : RoleStoreBase<InternalRole, Guid, IdentityUserRole<Guid>, IdentityRoleClaim<Guid>>
    {
        private readonly ICommandSender _sender;

        public InternalRoleStore(IdentityErrorDescriber describer, ICommandSender sender) 
            : base(describer) 
            => _sender = sender;

        public override async Task AddClaimAsync(InternalRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            await using var mem = new MemoryStream();
            await using var writer = new BinaryWriter(mem);
            claim.WriteTo(writer);
            
            await _sender.Send(new AddClaimToRoleCommand(role.Id, Convert.ToBase64String(mem.ToArray())), cancellationToken);
        }

        public override async Task<IList<Claim>> GetClaimsAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task RemoveClaimAsync(InternalRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IdentityResult> UpdateAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override IQueryable<InternalRole> Roles { get; }

        public override async Task<IdentityResult> CreateAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IdentityResult> DeleteAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<InternalRole> FindByIdAsync(string id, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<InternalRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();
    }
}