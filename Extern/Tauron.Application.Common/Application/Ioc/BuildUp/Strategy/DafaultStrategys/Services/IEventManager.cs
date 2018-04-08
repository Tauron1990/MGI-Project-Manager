#region

using System;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The EventManager interface.</summary>
    [PublicAPI]
    public interface IEventManager
    {
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
        /// <param name="errorTracer"></param>
        void AddEventHandler(string topic, Delegate handler, ErrorTracer errorTracer);

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
        /// <param name="errorTracer"></param>
        void AddEventHandler(string topic, MethodInfo handler, object target, ErrorTracer errorTracer);

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
        /// <param name="errorTracer"></param>
        void AddPublisher(string topic, EventInfo eventInfo, object publisher, ErrorTracer errorTracer);

        #endregion
    }
}