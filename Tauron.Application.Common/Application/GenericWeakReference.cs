using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application
{
	[DebuggerNonUserCode]
	[Serializable]
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public sealed class GenericWeakReference<TType> : WeakReference, IEquatable<GenericWeakReference<TType>>,
	                                                  IWeakReference
		where TType : class
	{
		public GenericWeakReference(TType target)
			: base(target)
		{
		}

		private GenericWeakReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public TType TypedTarget
		{
			get => (TType) Target;
		    set => Target = value;
		}
        
		public bool Equals(GenericWeakReference<TType> other)
		{
			var t1 = Target;
			var t2 = other?.Target;

			return t1?.Equals(t2) ?? t2 == null;
		}

		public override bool Equals(object obj)
		{
			var target = Target;
			object? temp = obj as GenericWeakReference<TType>;
			if (temp != null)
				return Equals(temp);

			return target?.Equals(obj) ?? obj == null;
		}

		public override int GetHashCode()
		{
			var target = Target;
			return target == null ? 0 : target.GetHashCode();
		}

		public static bool operator ==(GenericWeakReference<TType> left, GenericWeakReference<TType> right) => Equals(left, right);
        public static bool operator !=(GenericWeakReference<TType> left, GenericWeakReference<TType> right) => !Equals(left, right);
        public static bool operator ==(GenericWeakReference<TType> left, object right) => Equals(left, right);
        public static bool operator !=(GenericWeakReference<TType> left, object right) => !Equals(left, right);
	}
}