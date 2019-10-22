using System.Threading.Tasks;
using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services.Extensions;
using Tauron.MgiManager.Resources;
using Tauron.MgiManager.User.Service.Data;
using Tauron.MgiManager.User.Service.Data.Entitys;
using Tauron.MgiManager.User.Shared.Events;

namespace Tauron.MgiManager.User.Service.UserManager.Roles
{
    [CQRSHandler]
    public sealed class RoleStoreApplier : IEventHandler<ClaimToRoleAddedEvent>,
                                           IEventHandler<ClaimRemovedFromRoleEvent>
    {
        private readonly ILogger<RoleStoreApplier> _logger;
        private readonly UserDatabase _userDatabase;

        public RoleStoreApplier(ILogger<RoleStoreApplier> logger, UserDatabase userDatabase)
        {
            _logger = logger;
            _userDatabase = userDatabase;
        }

        public async Task Handle(ClaimToRoleAddedEvent message)
        {
            _logger.LogInformation(EventIds.UserManager.RoleManagment, $"{message.RoleId} -- Add Claim To Role");
            var role = await _userDatabase.UserRoles.FindAsync(message.RoleId);
            if (role == null)
            {
                _logger.LogWarning(EventIds.UserManager.RoleManagment, $"{message.RoleId} -- Role Not Found");
                role = new UserRole { Id = message.RoleId };

                await _userDatabase.UserRoles.AddAsync(role);
            }

            role.Claims.Add(new Claim
                            {
                                ClaimId = message.Id,
                                Data = message.Data
                            });

            await _userDatabase.SaveChangesAsync();
            _logger.LogInformation(EventIds.UserManager.RoleManagment, "Claim Added To Role");
        }

        public async Task Handle(ClaimRemovedFromRoleEvent message)
        {

        }
    }
}