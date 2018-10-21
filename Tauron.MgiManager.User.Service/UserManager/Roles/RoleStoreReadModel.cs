using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Core;
using Tauron.CQRS.Services.Extensions;
using Tauron.MgiManager.Resources;
using Tauron.MgiManager.User.Service.Data;
using Tauron.MgiManager.User.Shared.Dtos;
using Tauron.MgiManager.User.Shared.Querys;

namespace Tauron.MgiManager.User.Service.UserManager.Roles
{
    [CQRSHandler]
    public class RoleStoreReadModel : IReadModel<UserClaims, QueryRoleClaims>
    {
        private readonly IDispatcherClient _client;
        private readonly UserDatabase _userDatabase;
        private readonly ILogger<RoleStoreReadModel> _logger;

        public RoleStoreReadModel(IDispatcherClient client, UserDatabase userDatabase, ILogger<RoleStoreReadModel> logger)
        {
            _client = client;
            _userDatabase = userDatabase;
            _logger = logger;
        }

        public async Task ResolveQuery(QueryRoleClaims query, ServerDomainMessage serverDomainMessage)
        {
            _logger.LogInformation(EventIds.UserManager.RoleManagment, $"Query Role {query.RoleId} Claims");

            var role = await _userDatabase.UserRoles
                .Include(e => e.Claims)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == query.RoleId);

            var result = new UserClaims(role?.Id ?? Guid.Empty);

            if (role != null)
                result.Claims.AddRange(role.Claims.Select(e => new UserClaim(e.Data, e.Id)));
            else
                _logger.LogWarning(EventIds.UserManager.RoleManagment, $"Claims For {query.RoleId} Not Found: Returning Empty List");

            await _client.RespondToQuery(result, serverDomainMessage);
        }
    }
}