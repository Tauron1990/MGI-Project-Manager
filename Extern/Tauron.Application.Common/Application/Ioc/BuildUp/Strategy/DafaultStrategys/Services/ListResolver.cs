#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The list resolver.</summary>
    public class ListResolver : IResolver
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListResolver" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ListResolver" /> Klasse.
        /// </summary>
        /// <param name="resolvers">
        ///     The resolvers.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        public ListResolver(IEnumerable<IResolver> resolvers, Type target)
        {
            this.resolvers = resolvers;
            this.target = target;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object Create(ErrorTracer errorTracer)
        {
            try
            {
                errorTracer.Phase = "Injecting List for " + target;

                var closed = InjectorBaseConstants.List.MakeGenericType(target.GenericTypeArguments[0]);
                if (target.IsAssignableFrom(closed))
                {
                    var info = closed.GetMethod("Add");

                    var args = resolvers.Select(resolver => resolver.Create(errorTracer))
                        .TakeWhile(vtemp => !errorTracer.Exceptional).ToList();

                    if (errorTracer.Exceptional) return null;

                    var temp = Activator.CreateInstance(closed);

                    foreach (var o in args) info.Invoke(temp, o);

                    return temp;
                }

                errorTracer.Exceptional = true;
                errorTracer.Exception = new InvalidOperationException(target + " is Not Compatible");

                return null;
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        #endregion

        #region Fields

        private readonly IEnumerable<IResolver> resolvers;

        private readonly Type target;

        #endregion
    }
}