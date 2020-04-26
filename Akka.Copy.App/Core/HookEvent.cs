using System;

namespace Akka.Copy.App.Core
{
    public sealed class HookEvent
    {
        public Delegate Invoker { get; }
        public Type Target { get; }

        private HookEvent(Delegate invoker, Type target)
        {
            Invoker = invoker;
            Target = target;
        }

        public static HookEvent Create<TType>(Action<TType> action)
            => new HookEvent(action, typeof(TType));
    }
}