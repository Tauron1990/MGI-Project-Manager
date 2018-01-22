#region

using System;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The BaseHolder interface.</summary>
    [PublicAPI]
    public interface IBaseHolder
    {
        #region Public Properties

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [NotNull]
        string Name { get; set; }

        #endregion
    }

    /// <summary>The base holder.</summary>
    public static class BaseHolder
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get or add.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue GetOrAdd<TKey, TValue>([NotNull] ObjectContext context, [NotNull] Func<TValue> factory, [NotNull] string name)
            where TKey : class, IBaseHolder where TValue : class
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var instance = context.GetAll<TKey>().FirstOrDefault(holder => holder.Name == name) as TValue;

            if (instance != null) return instance;

            instance = factory();
            context.Register<TKey, TValue>(instance);
            return instance;
        }

        #endregion
    }

    /// <summary>
    ///     The base holder.
    /// </summary>
    /// <typeparam name="TValue">
    /// </typeparam>
    public abstract class BaseHolder<TValue> : IBaseHolder, IDisposable
        where TValue : class
    {
        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            var dipo = Value as IDisposable;
            if (dipo != null) dipo.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseHolder{TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="BaseHolder{TValue}" /> Klasse.
        ///     Initializes a new instance of the <see cref="BaseHolder{TValue}" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected BaseHolder([NotNull] TValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="BaseHolder{TValue}" /> class.
        ///     Finalisiert eine Instanz der <see cref="BaseHolder{TValue}" /> Klasse.
        /// </summary>
        ~BaseHolder()
        {
            Dispose();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        [NotNull]
        public TValue Value { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
}