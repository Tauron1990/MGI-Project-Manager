using System.Runtime.CompilerServices;
using CQRSlite.Snapshotting;
using JetBrains.Annotations;
using Tauron.CQRS.Services.Data;

namespace Tauron.CQRS.Services
{
    [PublicAPI]
    public class ServiceAggregateRoot : SnapshotAggregateRoot<AggregateData>
    {
        protected AggregateData Data { get; private set; } = new AggregateData();

        protected override AggregateData CreateSnapshot() => Data;

        protected override void RestoreFromSnapshot(AggregateData snapshot) => Data = snapshot;

        protected void SetValue(object value, [CallerMemberName] string name = null)
        {
            if(string.IsNullOrWhiteSpace(name)) return;

            Data.Objects[name] = value;
        }

        protected TType GetValue<TType>([CallerMemberName] string name = null)
        {
            if (name != null && Data.Objects.TryGetValue(name, out var value)) return value is TType type ? type : default;

            return default;
        }
    }
}