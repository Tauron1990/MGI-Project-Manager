using System;
using System.Runtime.CompilerServices;
using CQRSlite.Snapshotting;
using JetBrains.Annotations;
using Tauron.CQRS.Services.Data;

namespace Tauron.CQRS.Services
{
    [PublicAPI]
    public abstract class CoreAggregateRoot : SnapshotAggregateRoot<AggregateStade>
    {
        private object _lock = new object();

        protected internal AggregateStade AggregateStade { get; private set; } = new AggregateStade();

        protected override AggregateStade CreateSnapshot() => AggregateStade;

        protected override void RestoreFromSnapshot(AggregateStade snapshot)
        {
            if (snapshot == null) return;

            AggregateStade = snapshot;
        }

        protected TType GetValue<TType>([CallerMemberName] string name = null)
        {
            if (AggregateStade.Objects.TryGetValue(name ?? throw new ArgumentNullException(nameof(name)), out var value) && value is TType typedValue) return typedValue;
            return default;
        }

        protected void SetValue<TType>(TType value, [CallerMemberName] string name = null)
            => AggregateStade.Objects[name ?? throw new ArgumentNullException(nameof(name))] = value;
    }
}