// The file IResolver.cs is part of Tauron.Application.Common.
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

using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The Resolver interface.</summary>
    public interface IResolver
    {
        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        object Create(ErrorTracer errorTracer);

        #endregion
    }
}