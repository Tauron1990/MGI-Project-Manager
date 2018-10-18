using System;
using Tauron.CQRS.Services;

namespace Tauron.MgiManager.User.Shared.Events
{
    public sealed class UserCreatedEvent : BaseEvent
    {
        public string Name { get; set; }

        //[Remove]
        public string Hash { get; set; }

        //[Remove]
        public string Salt { get; set; }

        public override Guid Id { get; set; }

        public bool Error { get; set; }

        public string ErrorMessage { get; set; }

        public UserCreatedEvent(string name, string result)
        {
            ErrorMessage = result;
            Name = name;
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

        public override string ToString() => $"[Created User: {Name} -- {Id}]";
    }
}