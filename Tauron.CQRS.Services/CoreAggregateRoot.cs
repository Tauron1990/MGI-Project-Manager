using System;
using System.Runtime.CompilerServices;
using CQRSlite.Snapshotting;
using Tauron.CQRS.Services.Data;

namespace Tauron.CQRS.Services
{
    public abstract class CoreAggregateRoot : SnapshotAggregateRoot<AggregateStade>
    {
        private AggregateStade _aggregateStade;

        protected internal AggregateStade AggregateStade => _aggregateStade ?? new AggregateStade();

        protected override AggregateStade CreateSnapshot() => AggregateStade;

        protected override void RestoreFromSnapshot(AggregateStade snapshot) => _aggregateStade = snapshot;

        protected TType GetValue<TType>([CallerMemberName] string name = null)
        {
            if (_aggregateStade.Objects.TryGetValue(name ?? throw new ArgumentNullException(nameof(name)), out var value) && value is TType typedValue) return typedValue;
            return default;
        }

        protected void SetValue<TType>(TType value, [CallerMemberName] string name = null) 
            => _aggregateStade.Objects[name ?? throw new ArgumentNullException(nameof(name))] = value;
    }
}