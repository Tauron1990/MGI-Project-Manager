using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Queries;
using Microsoft.AspNetCore.Identity;
using Tauron.MgiManager.User.Shared.Command;
using Tauron.MgiManager.User.Shared.Dtos;
using Tauron.MgiManager.User.Shared.Querys;

namespace MgiManager.Data
{
    public sealed class InternalRoleStore : RoleStoreBase<InternalRole, Guid, IdentityUserRole<Guid>, IdentityRoleClaim<Guid>>
    {
        private readonly ICommandSender _sender;
        private readonly IQueryProcessor _queryProcessor;

        public InternalRoleStore(IdentityErrorDescriber describer, ICommandSender sender, IQueryProcessor queryProcessor) 
            : base(describer)
        {
            _sender = sender;
            _queryProcessor = queryProcessor;
        }

        public override async Task AddClaimAsync(InternalRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken()) 
            => await _sender.Send(new AddClaimToRoleCommand(role.Id, SerializeClaim(claim)), cancellationToken);

        public override async Task<IList<Claim>> GetClaimsAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            static Claim ReadClaim(UserClaim data)
            {
                using var mem = new MemoryStream(Convert.FromBase64String(data.Data))
                {
                    Position = 0
                };
                using var reader = new BinaryReader(mem);

                return new Claim(reader);
            }

            var result = await _queryProcessor.Query(new QueryRoleClaims(role.Id), cancellationToken);

            return result?.Claims.Select(ReadClaim).ToList();
        }

        public override async Task RemoveClaimAsync(InternalRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken()) 
            => await _sender.Send(new RemoveClaimFromRoleCommand(role.Id, await SerializeClaim(claim)), cancellationToken);

        public override async Task<IdentityResult> CreateAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IdentityResult> UpdateAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IdentityResult> DeleteAsync(InternalRole role, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<InternalRole> FindByIdAsync(string id, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<InternalRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override IQueryable<InternalRole> Roles { get; }

        private async Task<string> SerializeClaim(Claim claim)
        {
            await using var mem = new MemoryStream();
            await using var writer = new BinaryWriter(mem);
            claim.WriteTo(writer);

            return Convert.ToBase64String(mem.ToArray());
        }
    }
}