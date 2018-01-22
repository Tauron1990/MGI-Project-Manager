#region

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application
{
    /// <summary>The base object.</summary>
    [Serializable]
    public abstract class BaseObject : IContextHolder
    {
        #region Fields

        [NonSerialized]
        private ObjectContext _context;

        #endregion

        #region Explicit Interface Properties

        /// <summary>Gets or sets the context.</summary>
        /// <value>The context.</value>
        [CanBeNull]
        ObjectContext IContextHolder.Context
        {
            get => _context;

            set => _context = value;
        }

        #endregion
    }
}