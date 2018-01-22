// The file Constants.cs is part of Tauron.Application.Common.
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
// <copyright file="Constants.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The constants.</summary>
    public static class AopConstants
    {
        #region Constants

        /// <summary>The context metadata name.</summary>
        public const string ContextMetadataName = "ContextName";

        /// <summary>The default binding flags.</summary>
        public const BindingFlags DefaultBindingFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>The default export factory name.</summary>
        public const string DefaultExportFactoryName = "Internal";

        /// <summary>The event adder.</summary>
        public const string EventAdder = "add_";

        /// <summary>The event metod metadata name.</summary>
        public const string EventMetodMetadataName = "EventMethod";

        /// <summary>The event remover.</summary>
        public const string EventRemover = "remove_";

        /// <summary>The event topic metadata name.</summary>
        public const string EventTopicMetadataName = "EventTopic";

        /// <summary>The intercept metadata name.</summary>
        public const string InterceptMetadataName = "Intercept";

        /// <summary>The internal universal interceptor name.</summary>
        public const string InternalUniversalInterceptorName = "Internal@Universal";

        /// <summary>The liftime metadata name.</summary>
        public const string LiftimeMetadataName = "LiftimeMetadata";

        /// <summary>The parameter metadata name.</summary>
        public const string ParameterMetadataName = "Parameters";

        /// <summary>The property getter.</summary>
        public const string PropertyGetter = "get_";

        /// <summary>The property setter.</summary>
        public const string PropertySetter = "set_";

        #endregion
    }
}