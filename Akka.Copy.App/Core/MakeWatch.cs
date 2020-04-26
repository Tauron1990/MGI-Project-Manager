using Akka.Actor;

namespace Akka.Copy.App.Core
{
    public sealed class MakeWatch
    {
        public IActorRef ActorRef { get; }

        public MakeWatch(IActorRef actorRef) => ActorRef = actorRef;
    }
}