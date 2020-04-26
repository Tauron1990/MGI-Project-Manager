using Akka.Actor;

namespace Akka.Copy.App.Core
{
    public interface IEventActor
    {
        void Register(HookEvent hookEvent);

        void Watch(IActorRef actor);

        void Send(IActorRef actor, object send);
    }
}