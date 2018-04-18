#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The shared event.
    /// </summary>
    /// <typeparam name="TPayload">
    /// </typeparam>
    [PublicAPI]
    public abstract class SharedEvent<TPayload>
    {
        #region Fields

        /// <summary>The _handler list.</summary>
        private readonly WeakActionEvent<TPayload> _handlerList = new WeakActionEvent<TPayload>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The publish.
        /// </summary>
        /// <param name="content">
        ///     The content.
        /// </param>
        public virtual void Publish(TPayload content)
        {
            _handlerList.Invoke(content);
        }

        /// <summary>
        ///     The subscribe.
        /// </summary>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        public void Subscribe([NotNull] Action<TPayload> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlerList.Add(handler);
        }

        /// <summary>
        ///     The un subscribe.
        /// </summary>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un")]
        public void UnSubscribe([NotNull] Action<TPayload> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlerList.Remove(handler);
        }

        #endregion
    }

    /// <summary>The EventAggregator interface.</summary>
    public interface IEventAggregator
    {
        #region Public Methods and Operators

        /// <summary>The get event.</summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <returns>
        ///     The <see cref="TEventType" />.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        TEventType GetEvent<TEventType, TPayload>() where TEventType : SharedEvent<TPayload>, new();

        #endregion
    }

    /// <summary>The event aggregator.</summary>
    [Export(typeof(IEventAggregator))]
    [PublicAPI]
    public sealed class EventAggregator : IEventAggregator
    {
        #region Static Fields

        /// <summary>The _aggregator.</summary>
        private static IEventAggregator _aggregator;

        #endregion

        #region Fields

        /// <summary>The _events.</summary>
        private readonly Dictionary<Type, object> _events = new Dictionary<Type, object>();

        #endregion

        #region Public Properties

        /// <summary>Gets the aggregator.</summary>
        /// <value>The aggregator.</value>
        [NotNull]
        public static IEventAggregator Aggregator
        {
            get
            {
                if (_aggregator != null) return _aggregator;

                _aggregator =
                    (IEventAggregator) CommonApplication.Current.Container.Resolve(typeof(IEventAggregator), null);
                return _aggregator;
            }

            set
            {
                if (_aggregator == null)
                    _aggregator = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The get event.</summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <returns>
        ///     The <see cref="TEventType" />.
        /// </returns>
        public TEventType GetEvent<TEventType, TPayload>() where TEventType : SharedEvent<TPayload>, new()
        {
            var t                                   = typeof(TEventType);
            if (!_events.ContainsKey(t)) _events[t] = new TEventType();

            return (TEventType) _events[t];
        }

        #endregion
    }
}