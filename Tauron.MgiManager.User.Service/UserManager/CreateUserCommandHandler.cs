﻿using System;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Core;
using Tauron.CQRS.Services.Extensions;
using Tauron.CQRS.Services.Specifications;
using Tauron.MgiManager.Resources;
using Tauron.MgiManager.User.Service.Core;
using Tauron.MgiManager.User.Shared;
using Tauron.MgiManager.User.Shared.Command;
using Tauron.MgiManager.User.Shared.Events;

namespace Tauron.MgiManager.User.Service.UserManager
{
    [CQRSHandler]
    public class CreateUserCommandHandler : SpecificationCommandHandler<CreateUserCommand>
    {
        private readonly ISession _session;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(ISession session, ILogger<CreateUserCommandHandler> logger)
        {
            _session = session;
            _logger = logger;
        }

        public override async Task Handle(CreateUserCommand command, string error)
        {
            using (_logger.BeginScope(command))
            {
                var id = IdGenerator.Generator.NewGuid(UserNamespace.UserNameSpace, command.Name);

                try
                {
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        if (await _session.Exis<UserAggregate>(id))
                        {
                            _logger.LogWarning(EventIds.UserManager.UserCreation, $"User Name {command.Name} is in Use");
                            await _session.PublishEvent(new UserCreatedEvent(command.Name, UserServiceResources.CreateUserValidationFailed_Duplicate));
                            return;
                        }

                        var (hash, salt) = PasswordGenerator.Hash(command.Password);
                        var aggregate = new UserAggregate(id);

                        aggregate.CreateUser(command.Name, hash, salt);

                        await _session.Add(aggregate);
                        await _session.Commit();
                        _logger.LogInformation(EventIds.UserManager.UserCreation, $"{command.Name} User Created");
                    }
                    else
                    {
                        _logger.LogWarning(EventIds.UserManager.UserCreation, $"{command.Name} Validation Failed");
                        await _session.PublishEvent(new UserCreatedEvent(command.Name, error));
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(EventIds.UserManager.UserCreation, e, $"Error on Creating {command.Name}");
                }
            }
        }

        protected override ISpecification GetSpecification(GenericSpecification<CreateUserCommand> helper)
        {
            return helper
               .Simple(command => !string.IsNullOrWhiteSpace(command.Name), UserServiceResources.CreateUserValidationFailed_Name)
               .And(helper.Simple(command => !string.IsNullOrWhiteSpace(command.Password), UserServiceResources.CreateUserValidationFailed_Password));
        }
    }
}