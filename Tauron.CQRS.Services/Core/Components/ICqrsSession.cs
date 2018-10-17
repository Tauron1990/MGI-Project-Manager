using CQRSlite.Domain;
using CQRSlite.Events;

namespace Tauron.CQRS.Services.Core.Components
{
    public interface ICqrsSession : ISession
    {
        IEventPublisher EventPublisher { get; }
    }
}