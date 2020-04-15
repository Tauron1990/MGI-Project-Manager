// The file IIniSerializerConfiguration.cs is part of Tauron.Application.Files.
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

using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IIniSerializerConfiguration : ISerializerRootConfiguration, IConstructorConfig<IIniSerializerConfiguration>
    {
        [NotNull]
        IIniSectionSerializerConfiguration FromSection([NotNull] string name);

        [NotNull]
        ISerializerToMemberConfiguration<IIniSerializerConfiguration> MapSerializer<TToSerial>([NotNull] string memberName);
    }
}