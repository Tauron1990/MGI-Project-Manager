using System;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Core.Components;
using Tauron.MgiManager.User.Shared.Events;

namespace Tauron.MgiManager.User.Service.UserManager
{
    public sealed class UserAggregate : CoreAggregateRoot
    {
        public string Name
        {
            get => GetValue<string>();
            private set => SetValue(value);
        }

        public string Hash
        {
            get => GetValue<string>();
            private set => SetValue(value);
        }

        public string Salt
        {
            get => GetValue<string>();
            private set => SetValue(value);
        }

        public UserAggregate()
        {
            
        }

        public UserAggregate(Guid id)
        {
            Id = id;
        }

        private void Apply(UserCreatedEvent @event)
        {
            Name = @event.Name;
            Hash = @event.Hash;
            Salt = @event.Salt;
        }

        public void CreateUser(string name, string hash, string salt)
            => ApplyChange(new UserCreatedEvent(name, hash, salt, Id));
    }
}