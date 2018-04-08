#region

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private Dictionary<string, object> _singletons;

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

        protected T GetSingleton<T>(Func<T> factory, [CallerMemberName] string name = null)
            where T : class
        {
            if (name == null) return null;

            if(_singletons == null) _singletons = new Dictionary<string, object>();

            var result =_singletons.TryGetAndCast<T>(name);
            if (result != null) return result;

            T value = factory();
            _singletons[name] = factory;
            return value;
        }

        #endregion
    }
}