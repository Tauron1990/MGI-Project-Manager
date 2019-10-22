using System;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Domain;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services.Core;
using Tauron.CQRS.Services.Extensions;
using Tauron.MgiManager.Resources;
using Tauron.MgiManager.User.Shared;
using Tauron.MgiManager.User.Shared.Command;
using Tauron.MgiManager.User.Shared.Events;

namespace Tauron.MgiManager.User.Service.UserManager.Roles
{
    [CQRSHandler]
    public sealed class RoleStoreHandler : ICommandHandler<AddClaimToRoleCommand>, ICommandHandler<RemoveClaimFromRoleCommand>
    {
        private readonly ILogger<RoleStoreHandler> _logger;
        private readonly ISession _session;

        public RoleStoreHandler(ILogger<RoleStoreHandler> logger, ISession session)
        {
            _logger = logger;
            _session = session;
        }

        public async Task Handle(AddClaimToRoleCommand message)
        {
            _logger.LogInformation(EventIds.UserManager.RoleManagment, "Adding Claim To Role");
            try
            {
                if(string.IsNullOrWhiteSpace(message.Data)) return;

                var aggregate = await _session.GetOrAdd<RoleClaimsAggregate>(IdGenerator.Generator.NewGuid(UserNamespace.ClaimToRole, message.Role.ToString()));

                aggregate.PublicApplyEvent(new ClaimToRoleAddedEvent(aggregate.Id, message.Data, message.Role));

                await _session.Commit();
                _logger.LogInformation(EventIds.UserManager.RoleManagment, $"{message.Role}: Claim Added");
            }
            catch (Exception e)
            {
                _logger.LogError(EventIds.UserManager.RoleManagment, e, "Error On Add Claim");
            }
        }

        public async Task Handle(RemoveClaimFromRoleCommand message)
        {
            _logger.LogInformation(EventIds.UserManager.RoleManagment, "Remove Claim From Role");
            try
            {
                if (string.IsNullOrWhiteSpace(message.Data)) return;

                var aggregate = await _session.Get<RoleClaimsAggregate>(IdGenerator.Generator.NewGuid(UserNamespace.ClaimToRole, message.Role.ToString()));

                aggregate.PublicApplyEvent(new ClaimRemovedFromRoleEvent(aggregate.Id, message.Data, message.Role));

                await _session.Commit();
                _logger.LogInformation(EventIds.UserManager.RoleManagment, $"{message.Role}: Claim Removed");
            }
            catch (Exception e)
            {
                _logger.LogError(EventIds.UserManager.RoleManagment, e, "Error On Remove Claim");
            }
        }
    }
}