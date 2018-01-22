#region

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using JetBrains.Annotations;

#endregion

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
			get
			{
				object target = Target;
				return target == null ? null : (TType) target;
			}
			set { Target = value; }
		}

		#region IEquatable<GenericWeakReference<TType>> Members

		public bool Equals(GenericWeakReference<TType> other)
		{
			object t1 = Target;
			object t2 = ReferenceEquals(other, null) ? null : other.Target;

			return t1 == null ? t2 == null : t1.Equals(t2);
		}

		#endregion

		public override bool Equals(object obj)
		{
			object target = Target;
			var temp = obj.As<GenericWeakReference<TType>>();
			if (temp != null)
				return Equals(temp);

			return target == null ? obj == null : target.Equals(obj);
		}

		public override int GetHashCode()
		{
			object target = Target;
			return target == null ? 0 : target.GetHashCode();
		}

		public static bool operator ==(GenericWeakReference<TType> left, GenericWeakReference<TType> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(GenericWeakReference<TType> left, GenericWeakReference<TType> right)
		{
			return !Equals(left, right);
		}

		public static bool operator ==(GenericWeakReference<TType> left, object right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(GenericWeakReference<TType> left, object right)
		{
			return !Equals(left, right);
		}
	}
}