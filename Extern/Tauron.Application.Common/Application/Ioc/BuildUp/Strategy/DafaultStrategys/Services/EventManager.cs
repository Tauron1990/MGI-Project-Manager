// The file EventManager.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The event manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The event manager.</summary>
    public class EventManager : IEventManager
    {
        /// <summary>The event entry.</summary>
        private class EventEntry : IWeakReference
        {
            /// <summary>The handler entry.</summary>
            private class HandlerEntry : IWeakReference
            {
                #region Enums

                /// <summary>The invoke type.</summary>
                private enum InvokeType
                {
                    /// <summary>The zero.</summary>
                    Zero = 0,

                    /// <summary>The one.</summary>
                    One = 1,

                    /// <summary>The two.</summary>
                    Two = 2
                }

                #endregion

                private readonly Delegate _dDelegate;

                #region Public Properties

                /// <summary>Gets a value indicating whether is alive.</summary>
                /// <value>The is alive.</value>
                public bool IsAlive => _delegate.IsAlive;

                #endregion

                #region Public Methods and Operators

                /// <summary>
                ///     The invoke.
                /// </summary>
                /// <param name="sender">
                ///     The sender.
                /// </param>
                /// <param name="args">
                ///     The args.
                /// </param>
                /// <exception cref="ArgumentOutOfRangeException">
                /// </exception>
                public void Invoke([NotNull] object sender, [NotNull] object args)
                {
                    if (sender == null) throw new ArgumentNullException(nameof(sender));
                    if (args == null) throw new ArgumentNullException(nameof(args));
                    switch (_type)
                    {
                        case InvokeType.Zero:
                            _delegate.Invoke();
                            break;
                        case InvokeType.One:
                            _delegate.Invoke(args);
                            break;
                        case InvokeType.Two:
                            _delegate.Invoke(sender, args);
                            break;
                    }
                }

                #endregion

                #region Fields

                /// <summary>The _delegate.</summary>
                private readonly WeakDelegate _delegate;

                /// <summary>The _type.</summary>
                private readonly InvokeType _type;

                #endregion

                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the <see cref="HandlerEntry" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="HandlerEntry" /> Klasse.
                ///     Initializes a new instance of the <see cref="HandlerEntry" /> class.
                /// </summary>
                /// <param name="dDelegate">
                ///     The d delegate.
                /// </param>
                public HandlerEntry([NotNull] Delegate dDelegate)
                {
                    if (dDelegate == null) throw new ArgumentNullException(nameof(dDelegate));
                    _dDelegate = dDelegate;
                    _type = (InvokeType) dDelegate.Method.GetParameters().Length;
                    _delegate = new WeakDelegate(dDelegate);
                }

                /// <summary>
                ///     Initializes a new instance of the <see cref="HandlerEntry" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="HandlerEntry" /> Klasse.
                ///     Initializes a new instance of the <see cref="HandlerEntry" /> class.
                /// </summary>
                /// <param name="methodInfo">
                ///     The method info.
                /// </param>
                /// <param name="target">
                ///     The target.
                /// </param>
                public HandlerEntry([NotNull] MethodBase methodInfo, [NotNull] object target)
                {
                    if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
                    if (target == null) throw new ArgumentNullException(nameof(target));
                    _type = (InvokeType) methodInfo.GetParameters().Length;
                    _delegate = new WeakDelegate(methodInfo, target);
                }

                #endregion
            }

            #region Static Fields

            /// <summary>The handlermethod.</summary>
            private static readonly MethodInfo Handlermethod = typeof(EventEntry).GetMethod(
                "GeneralHandler",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="EventEntry" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="EventEntry" /> Klasse.
            ///     Initializes a new instance of the <see cref="EventEntry" /> class.
            /// </summary>
            /// <param name="topic">
            ///     The _topic.
            /// </param>
            public EventEntry([NotNull] string topic)
            {
                if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
                Topic = topic;
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The general handler.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="args">
            ///     The args.
            /// </param>
            [UsedImplicitly(ImplicitUseKindFlags.Access)]
            private void GeneralHandler(object sender, EventArgs args)
            {
                foreach (var weakDelegate in _handler) weakDelegate.Invoke(sender, args);
            }

            #endregion

            #region Fields

            /// <summary>The _handler.</summary>
            private readonly WeakReferenceCollection<HandlerEntry> _handler =
                new WeakReferenceCollection<HandlerEntry>();

            /// <summary>The _publisher.</summary>
            private readonly WeakCollection<object> _publisher = new WeakCollection<object>();

            #endregion

            #region Public Properties

            /// <summary>Gets the _topic.</summary>
            /// <value>The _topic.</value>
            public string Topic { get; }

            /// <summary>Gets a value indicating whether is alive.</summary>
            /// <value>The is alive.</value>
            public bool IsAlive => _handler.Count != 0 && _publisher.Count != 0;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The add publisher.
            /// </summary>
            /// <param name="info">
            ///     The info.
            /// </param>
            /// <param name="publisher">
            ///     The publisher.
            /// </param>
            public void AddPublisher([NotNull] EventInfo info, [NotNull] object publisher, [NotNull] ErrorTracer errorTracer)
            {
                if (info == null) throw new ArgumentNullException(nameof(info));
                if (publisher == null) throw new ArgumentNullException(nameof(publisher));
                if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
                errorTracer.Phase = "Adding Publisher " + Topic;
                info.AddEventHandler(publisher, Delegate.CreateDelegate(info.EventHandlerType, this, Handlermethod));
                _publisher.Add(publisher);
            }

            /// <summary>
            ///     The addhandler.
            /// </summary>
            /// <param name="dDelegate">
            ///     The d delegate.
            /// </param>
            public void Addhandler([NotNull] Delegate dDelegate, [NotNull] ErrorTracer errorTracer)
            {
                if (dDelegate == null) throw new ArgumentNullException(nameof(dDelegate));
                if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
                errorTracer.Phase = "Adding Handler " + Topic;
                _handler.Add(new HandlerEntry(dDelegate));
            }

            /// <summary>
            ///     The addhandler.
            /// </summary>
            /// <param name="dDelegate">
            ///     The d delegate.
            /// </param>
            /// <param name="target">
            ///     The target.
            /// </param>
            public void Addhandler([NotNull] MethodInfo dDelegate, [NotNull] object target, [NotNull] ErrorTracer errorTracer)
            {
                if (dDelegate == null) throw new ArgumentNullException(nameof(dDelegate));
                if (target == null) throw new ArgumentNullException(nameof(target));
                if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
                errorTracer.Phase = "Adding Handler " + Topic;
                _handler.Add(new HandlerEntry(dDelegate, target));
            }

            #endregion
        }

        #region Fields

        /// <summary>The _entrys.</summary>
        private readonly WeakReferenceCollection<EventEntry> _entrys = new WeakReferenceCollection<EventEntry>();

        #endregion

        #region Methods

        /// <summary>
        ///     The event action.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="entryAction">
        ///     The entry action.
        /// </param>
        /// <param name="errorTracer"></param>
        private void EventAction([NotNull] string topic, [NotNull] Action<EventEntry> entryAction, ErrorTracer errorTracer)
        {
            if (entryAction == null) throw new ArgumentNullException(nameof(entryAction));
            if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
            errorTracer.Phase = "Resolve Topic " + topic;

            var entry = _entrys.FirstOrDefault(ent => ent.Topic == topic);
            var add = false;
            if (entry == null)
            {
                add = true;
                entry = new EventEntry(topic);
            }

            entryAction(entry);

            if (add) _entrys.Add(entry);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add handler.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        public void AddEventHandler([NotNull] string topic, [NotNull] Delegate handler, [NotNull] ErrorTracer errorTracer)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
            if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(topic));
            lock (this)
            {
                EventAction(topic, ev => ev.Addhandler(handler, errorTracer), errorTracer);
            }
        }

        /// <summary>
        ///     The add handler.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        public void AddEventHandler(string topic, MethodInfo handler, object target, ErrorTracer errorTracer)
        {
            EventAction(topic, entry => entry.Addhandler(handler, target, errorTracer), errorTracer);
        }

        /// <summary>
        ///     The add publisher.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="eventInfo">
        ///     The event info.
        /// </param>
        /// <param name="publisher">
        ///     The publisher.
        /// </param>
        public void AddPublisher(string topic, EventInfo eventInfo, object publisher, ErrorTracer errorTracer)
        {
            lock (this)
            {
                EventAction(topic, ev => ev.AddPublisher(eventInfo, publisher, errorTracer), errorTracer);
            }
        }

        #endregion
    }
}