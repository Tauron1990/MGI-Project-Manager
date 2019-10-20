using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MgiManager.Data
{
    public sealed class InternalUserStore : UserStoreBase<InternalUser, 
        InternalRole, Guid, IdentityUserClaim<Guid>, 
        IdentityUserRole<Guid>, IdentityUserLogin<Guid>, 
        IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>
    {
        public InternalUserStore(IdentityErrorDescriber describer) : base(describer)
        {
        }

        public override async Task AddClaimsAsync(InternalUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IList<Claim>> GetClaimsAsync(InternalUser user, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IList<InternalUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task RemoveClaimsAsync(InternalUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task ReplaceClaimAsync(InternalUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task RemoveLoginAsync(InternalUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        protected override async Task RemoveUserTokenAsync(IdentityUserToken<Guid> token) => throw new NotImplementedException();

        public override async Task AddLoginAsync(InternalUser user, UserLoginInfo login, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IList<UserLoginInfo>> GetLoginsAsync(InternalUser user, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        protected override async Task AddUserTokenAsync(IdentityUserToken<Guid> token) => throw new NotImplementedException();

        public override async Task<IdentityResult> CreateAsync(InternalUser user, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IdentityResult> DeleteAsync(InternalUser user, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<InternalUser> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<InternalUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IdentityResult> UpdateAsync(InternalUser user, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override IQueryable<InternalUser> Users { get; }

        protected override async Task<IdentityUserToken<Guid>> FindTokenAsync(InternalUser user, string loginProvider, string name, CancellationToken cancellationToken) => throw new NotImplementedException();

        protected override async Task<InternalUser> FindUserAsync(Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();

        protected override async Task<IdentityUserLogin<Guid>> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();

        protected override async Task<IdentityUserLogin<Guid>> FindUserLoginAsync(Guid userId, string loginProvider, string providerKey, CancellationToken cancellationToken) => throw new NotImplementedException();

        public override async Task<InternalUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task AddToRoleAsync(InternalUser user, string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IList<string>> GetRolesAsync(InternalUser user, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<IList<InternalUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task<bool> IsInRoleAsync(InternalUser user, string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        public override async Task RemoveFromRoleAsync(InternalUser user, string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

        protected override async Task<InternalRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken) => throw new NotImplementedException();

        protected override async Task<IdentityUserRole<Guid>> FindUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}