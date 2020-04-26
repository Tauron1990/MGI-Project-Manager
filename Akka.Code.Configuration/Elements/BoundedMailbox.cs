using System;
using JetBrains.Annotations;

namespace Akka.Code.Configuration.Elements
{
    [PublicAPI]
    public sealed class BoundedMailbox : ConfigurationElement
    {
        public int Capacity
        {
            get => Get<int>("mailbox-capacity");
            set => Set(value, "mailbox-capacity");
        }

        public TimeSpan Timeout
        {
            get => Get<TimeSpan>("mailbox-push-timeout-time");
            set => Set(value, "mailbox-push-timeout-time");
        }

        public AkkaType MailboxType
        {
            get => Get<string>("mailbox-type");
            private set => Set(value, "mailbox-type");
        }

        public BoundedMailbox() 
            => MailboxType = "Akka.Dispatch.BoundedMailbox, Akka";

        public BoundedMailbox(int capacity)
            : this()
        {
            Capacity = capacity;
            Timeout = new TimeSpan(-1);
        }

        public BoundedMailbox(int capacity, TimeSpan timeout)
            : this()
        {
            Capacity = capacity;
            Timeout = timeout;
        }
    }
}