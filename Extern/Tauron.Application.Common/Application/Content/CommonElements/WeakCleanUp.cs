#region

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using NLog;

#endregion

namespace Tauron.Application
{
    /// <summary>The weak delegate.</summary>
    [PublicAPI]
    public sealed class WeakDelegate : IWeakReference, IEquatable<WeakDelegate>
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether is alive.</summary>
        /// <value>The is alive.</value>
        public bool IsAlive => _reference == null || _reference.IsAlive;

        #endregion

        #region Fields

        /// <summary>The _method.</summary>
        private readonly MethodBase _method;

        /// <summary>The _reference.</summary>
        private readonly WeakReference _reference;

        #endregion

        #region Constructors and Destructors

        public WeakDelegate([NotNull] Delegate @delegate)
        {
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            _method = @delegate.Method;

            if (!_method.IsStatic) _reference = new WeakReference(@delegate.Target);
        }

        public WeakDelegate([NotNull] MethodBase methodInfo, [NotNull] object target)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _method = methodInfo;
            _reference = new WeakReference(target);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="other">
        ///     The other.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Equals(WeakDelegate other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return other._reference.Target == _reference.Target && other._method == _method;
        }

        /// <summary>The ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator ==(WeakDelegate left, WeakDelegate right)
        {
            var leftnull = ReferenceEquals(left, null);
            var rightNull = ReferenceEquals(right, null);

            return !leftnull ? left.Equals(right) : rightNull;
        }

        /// <summary>The !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(WeakDelegate left, WeakDelegate right)
        {
            var leftnull = ReferenceEquals(left, null);
            var rightNull = ReferenceEquals(right, null);

            if (!leftnull) return !left.Equals(right);
            return !rightNull;
        }

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            return obj is WeakDelegate && Equals((WeakDelegate) obj);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                object target;
                return (((target = _reference.Target) != null ? target.GetHashCode() : 0) * 397)
                       ^ _method.GetHashCode();
            }
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="parms">
        ///     The parms.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        public object Invoke([CanBeNull] params object[] parms)
        {
            if (_method.IsStatic) return _method.Invoke(null, parms);

            var target = _reference.Target;
            return target == null ? null : _method.Invoke(target, parms);
        }

        #endregion
    }

    /// <summary>The weak clean up.</summary>
    [PublicAPI]
    public static class WeakCleanUp
    {
        #region Constants

        /// <summary>WeakCleanUpExceptionPolicy.</summary>
        public const string WeakCleanUpExceptionPolicy = "WeakCleanUpExceptionPolicy";

        #endregion

        #region Static Fields

        /// <summary>The actions.</summary>
        private static readonly List<WeakDelegate> Actions = Initialize();

        private static Timer _timer;

        private static readonly Logger Logger = LogManager.GetLogger(nameof(WeakCleanUp));
        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The register action.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        public static void RegisterAction([NotNull] Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            lock (Actions)
            {
                Actions.Add(new WeakDelegate(action));
            }
        }

        #endregion

        #region Methods

        /// <summary>The initialize.</summary>
        /// <returns>The List.</returns>
        private static List<WeakDelegate> Initialize()
        {
            _timer = new Timer(InvokeCleanUp, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return new List<WeakDelegate>();
        }

        /// <summary>The invoke clean up.</summary>
        private static void InvokeCleanUp(object state)
        {
            lock (Actions)
            {
                var dead = new List<WeakDelegate>();
                foreach (var weakDelegate in Actions.ToArray())
                    if (weakDelegate != null && weakDelegate.IsAlive)
                        try
                        {
                            weakDelegate.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    else dead.Add(weakDelegate);

                dead.ForEach(del => Actions.Remove(del));
            }
        }

        #endregion
    }
}