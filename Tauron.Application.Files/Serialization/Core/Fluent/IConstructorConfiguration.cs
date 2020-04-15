// The file IConstructorConfiguration.cs is part of Tauron.Application.Files.
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
//  along with Tauron.Application.Files If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    [PublicAPI]
    public interface IConstructorConfiguration<out TConfig>
    {
        TConfig Apply();

        [NotNull]
        IConstructorConfiguration<TConfig> WhithConstructor([NotNull] ConstructorInfo info);

        [NotNull]
        IConstructorConfiguration<TConfig> WithCusomConstructor([NotNull] Func<object, object> constructor);

        [NotNull]
        IConstructorConfiguration<TConfig> WithObject([NotNull] object parm);
    }
}