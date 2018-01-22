#region

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The policy list.</summary>
    [PublicAPI]
    public class PolicyList
    {
        #region Fields

        /// <summary>The _list.</summary>
        private readonly GroupDictionary<Type, IPolicy> _list = new GroupDictionary<Type, IPolicy>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="policy">
        ///     The policy.
        /// </param>
        /// <typeparam name="TPolicy">
        /// </typeparam>
        public void Add<TPolicy>([NotNull] TPolicy policy) where TPolicy : IPolicy
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));
            _list.Add(typeof(TPolicy), policy);
        }

        /// <summary>The get.</summary>
        /// <typeparam name="TPolicy"></typeparam>
        /// <returns>
        ///     The <see cref="TPolicy" />.
        /// </returns>
        public TPolicy Get<TPolicy>()
        {
            ICollection<IPolicy> policies;
            if (!_list.TryGetValue(typeof(TPolicy), out policies)) return default(TPolicy);

            return (TPolicy) policies.Last();
        }

        public IEnumerable<TPolicy> GetAll<TPolicy>()
        {
            ICollection<IPolicy> policies;
            if (!_list.TryGetValue(typeof(TPolicy), out policies)) return Enumerable.Empty<TPolicy>();

            return policies.Cast<TPolicy>();
        }

        /// <summary>The remove.</summary>
        /// <typeparam name="TPolicy"></typeparam>
        public void Remove<TPolicy>()
        {
            _list.Remove(typeof(TPolicy));
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="policy">
        ///     The policy.
        /// </param>
        /// <typeparam name="TPolicy">
        /// </typeparam>
        public void Remove<TPolicy>(TPolicy policy) where TPolicy : IPolicy
        {
            _list.RemoveValue(policy);
        }

        #endregion
    }
}