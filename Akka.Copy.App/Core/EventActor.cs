using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Akka.Copy.App.Core
{
    public sealed class EventActor : UntypedActor
    {
        private sealed class HookEventActor : IEventActor
        {
            private readonly IActorRef _actorRef;

            public HookEventActor(IActorRef actorRef) 
                => _actorRef = actorRef;

            public void Register(HookEvent hookEvent) 
                => _actorRef.Tell(hookEvent);

            public void Watch(IActorRef actor) 
                => _actorRef.Tell(new MakeWatch(actor));

            public void Send(IActorRef actor, object send) 
                => actor.Tell(send, _actorRef);
        }

        public static IEventActor Create(ActorSystem system) 
            => new HookEventActor(system.ActorOf<EventActor>());

        private readonly Dictionary<Type, Delegate> _registrations = new Dictionary<Type, Delegate>();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case MakeWatch watch:
                    Context.Watch(watch.ActorRef);
                    break;
                case HookEvent hookEvent:
                    if (_registrations.TryGetValue(hookEvent.Target, out var del))
                        del = Delegate.Combine(del, hookEvent.Invoker);
                    else
                        del = hookEvent.Invoker;

                    _registrations[hookEvent.Target] = del;
                    break;
                default:
                    var msgType = message.GetType();
                    if (_registrations.TryGetValue(msgType, out var callDel))
                        callDel.DynamicInvoke(message);
                    else
                        Unhandled(message);
                    break;
            }
        }
    }
}