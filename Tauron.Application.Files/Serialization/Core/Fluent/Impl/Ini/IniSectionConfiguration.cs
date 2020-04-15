using System;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class IniSectionConfiguration : IIniSectionSerializerConfiguration
    {
        private readonly IIniSerializerConfiguration _configuration;
        private readonly SimpleMapper<IniContext> _mapper;
        private readonly string _section;
        private readonly Type _targeType;

        public IniSectionConfiguration([NotNull] string section, [NotNull] SimpleMapper<IniContext> mapper,
            [NotNull] IIniSerializerConfiguration configuration, [NotNull] Type targeType)
        {
            _section = section;
            _mapper = mapper;
            _configuration = configuration;
            _targeType = targeType;
        }

        public IIniKeySerializerConfiguration WithSingleKey()
        {
            return new IniKeyConfiguration(_section, _configuration, _mapper, true, _targeType);
        }

        public IIniKeySerializerConfiguration WithListKey()
        {
            return new IniKeyConfiguration(_section, _configuration, _mapper, false, _targeType);
        }
    }
}