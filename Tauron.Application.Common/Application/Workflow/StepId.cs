using System.Diagnostics;
using JetBrains.Annotations;

namespace Tauron.Application.Workflow
{
    [PublicAPI]
    public struct StepId
    {
        //public static readonly StepId Null = new StepId();

        public static readonly StepId Invalid = new StepId("Invalid");
        public static readonly StepId None = new StepId("None");
        public static readonly StepId Finish = new StepId("Finish");
        public static readonly StepId Loop = new StepId("Loop");
        public static readonly StepId LoopEnd = new StepId("LoopEnd");
        public static readonly StepId Skip = new StepId("Skip");

        [DebuggerStepThrough]
        public override int GetHashCode() => Name.GetHashCode();

        public StepId([NotNull] string name) : this()
        {
            Argument.NotNull(name, nameof(name));
            Name = name;
        }

        [NotNull]
        public string Name { get; }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (!(obj is StepId)) return false;

            return ((StepId) obj).Name == Name;
        }

        public static bool operator ==(StepId id1, StepId id2) => id1.Name == id2.Name;

        public static bool operator !=(StepId id1, StepId id2) => id1.Name != id2.Name;

        [DebuggerStepThrough]
        public override string ToString() => Name;
    }
}