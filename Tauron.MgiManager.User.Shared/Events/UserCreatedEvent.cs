using System;
using Tauron.CQRS.Services;

namespace Tauron.MgiManager.User.Shared.Events
{
    public sealed class UserCreatedEvent : BaseEvent
    {
        public string Name { get; set; }

        [Remove]
        public string Hash { get; set; }

        [Remove]
        public string Salt { get; set; }

        public override Guid Id { get; set; }

        public bool Error { get; set; }

        public UserCreatedEvent(string result)
        {
            Name = result;
            Id = Guid.NewGuid();
            Error = true;
        }

        public UserCreatedEvent(string name, string hash, string salt, Guid id)
        {
            Name = name;
            Hash = hash;
            Salt = salt;
            Id = id;
            Error = false;
        }

        public UserCreatedEvent()
        {
            
        }
    }
}