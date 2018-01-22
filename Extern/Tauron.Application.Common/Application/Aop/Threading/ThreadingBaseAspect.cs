#region

using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The threading base aspect.</summary>
    public abstract class ThreadingBaseAspect : AspectBaseAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThreadingBaseAspect" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ThreadingBaseAspect" /> Klasse.
        ///     Initializes a new instance of the <see cref="ThreadingBaseAspect" /> class.
        /// </summary>
        protected ThreadingBaseAspect()
        {
            Order = 200;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the holder name.</summary>
        /// <value>The holder name.</value>
        [NotNull]
        public string HolderName { get; set; }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether is initialized.</summary>
        /// <value>The is initialized.</value>
        protected bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        protected internal override void Initialize(object target, ObjectContext context, string contextName)
        {
            base.Initialize(target, context, contextName);

            IsInitialized = true;
        }

        #endregion
    }
}