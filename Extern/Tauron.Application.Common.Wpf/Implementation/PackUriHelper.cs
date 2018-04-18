// The file PackUriHelper.cs is part of Tauron.Application.Common.Wpf.
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
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackUriHelper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The pack uri helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Implementation
{
    // [DebuggerNonUserCode]
    /// <summary>The pack uri helper.</summary>
    [PublicAPI]
    [Export(typeof(IPackUriHelper))]
    public class PackUriHelper : IPackUriHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get string.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetString(string pack)
        {
            return GetString(pack, Assembly.GetCallingAssembly().GetName().Name, false);
        }

        /// <summary>
        ///     The get string.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <param name="full">
        ///     The full.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetString(string pack, string assembly, bool full)
        {
            if (assembly == null) return pack;

            var fullstring = full ? "pack://application:,,," : string.Empty;
            return string.Format("{0}/{1};component/{2}", fullstring, assembly, pack);
        }

        /// <summary>
        ///     The get uri.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="Uri" />.
        /// </returns>
        public Uri GetUri(string pack)
        {
            return GetUri(pack, Assembly.GetCallingAssembly().GetName().Name, false);
        }

        /// <summary>
        ///     The get uri.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <param name="full">
        ///     The full.
        /// </param>
        /// <returns>
        ///     The <see cref="Uri" />.
        /// </returns>
        public Uri GetUri(string pack, string assembly, bool full)
        {
            var compledpack = GetString(pack, assembly, full);
            var uriKind     = full ? UriKind.Absolute : UriKind.Relative;

            return new Uri(compledpack, uriKind);
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public T Load<T>(string pack) where T : class
        {
            return Load<T>(pack, Assembly.GetCallingAssembly().GetName().Name);
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public T Load<T>(string pack, string assembly) where T : class
        {
            return (T) System.Windows.Application.LoadComponent(GetUri(pack, assembly, false));
        }

        /// <summary>
        ///     The load stream.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Stream LoadStream(string pack)
        {
            return LoadStream(pack, Assembly.GetCallingAssembly().GetName().Name);
        }

        /// <summary>
        ///     The load stream.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Stream LoadStream(string pack, string assembly)
        {
            var info = System.Windows.Application.GetResourceStream(GetUri(pack, assembly, true));
            if (info != null) return info.Stream;

            throw new InvalidOperationException();
        }

        #endregion
    }
}