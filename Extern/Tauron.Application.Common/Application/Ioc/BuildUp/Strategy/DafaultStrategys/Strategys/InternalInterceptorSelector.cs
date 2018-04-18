// The file InternalInterceptorSelector.cs is part of Tauron.Application.Common.
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
// <copyright file="InternalInterceptorSelector.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The internal interceptor selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The internal interceptor selector.</summary>
    internal class InternalInterceptorSelector : IInterceptorSelector
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The select interceptors.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="interceptors">
        ///     The interceptors.
        /// </param>
        /// <returns>
        ///     The <see cref="IInterceptor[]" />.
        /// </returns>
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var name = method.Name;
            if (method.IsSpecialName)
            {
                if (name.StartsWith(AopConstants.PropertyGetter, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.PropertyGetter.Length);

                if (name.StartsWith(AopConstants.PropertySetter, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.PropertySetter.Length);

                if (name.StartsWith(AopConstants.EventAdder, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.EventAdder.Length);

                if (name.StartsWith(AopConstants.EventRemover, StringComparison.Ordinal)) name = name.Remove(0, AopConstants.EventRemover.Length);
            }

            return interceptors.Where(
                                      inter =>
                                      {
                                          var sinter = inter as ISpecificInterceptor;
                                          if (sinter != null)
                                              return sinter.Name == name ||
                                                     sinter.Name == AopConstants.InternalUniversalInterceptorName;

                                          return true;
                                      })
                               .OrderBy(
                                        inter =>
                                        {
                                            var sinter = inter as ISpecificInterceptor;
                                            return sinter == null ? 0 : sinter.Order;
                                        })
                               .ToArray();
        }

        #endregion
    }
}