using System;
using System.Threading.Tasks;
using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services.Extensions;
using Tauron.MgiManager.Resources;
using Tauron.MgiManager.User.Service.Data;
using Tauron.MgiManager.User.Service.Data.Entitys;
using Tauron.MgiManager.User.Shared.Events;

namespace Tauron.MgiManager.User.Service.UserManager
{
    [CQRSHandler]
    public class UserDataApplier : IEventHandler<UserCreatedEvent>
    {
        private readonly UserDatabase _database;
        private readonly ILogger<UserDataApplier> _logger;

        public UserDataApplier(UserDatabase database, ILogger<UserDataApplier> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task Handle(UserCreatedEvent message)
        {
            if (message.Error)
                return;

            using (_logger.BeginScope(message))
            {
                try
                {
                    await _database.AddAsync(new ApplicationUser
                    {
                        Id = message.Id,
                        Name = message.Name,
                        Password = message.Hash,
                        Salt = message.Salt
                    });

                    await _database.SaveChangesAsync();

                    _logger.LogInformation(EventIds.UserManager.UserManagment, $"User {message.Name} added to the Readmodel");
                }
                catch (Exception e)
                {
                    _logger.LogError(EventIds.UserManager.UserManagment, e, $"User {message.Name} adding Failed");
                }
            }
        }
    }
}