using System;
using System.Collections.Generic;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class IniKeyConfiguration : IIniKeySerializerConfiguration
    {
        private readonly IIniSerializerConfiguration _configuration;
        private readonly bool _isSingle;
        private readonly SimpleMapper<IniContext> _mapper;
        private readonly string _section;
        private readonly Type _targetType;
        private SimpleConverter<string>? _converter;
        private string? _key;
        private SimpleConverter<IEnumerable<string>>? _listConverter;

        private string? _member;

        public IniKeyConfiguration(string section, IIniSerializerConfiguration configuration,
            SimpleMapper<IniContext> mapper, bool isSingle, Type targetType)
        {
            _section = section;
            _configuration = configuration;
            _mapper = mapper;
            _isSingle = isSingle;
            _targetType = targetType;
        }


        public IIniKeySerializerConfiguration WithMember(string member)
        {
            _member = member;
            return this;
        }

        public IIniKeySerializerConfiguration WithKey(string? name)
        {
            _key = name;
            return this;
        }

        public IIniKeySerializerConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }

        public IIniKeySerializerConfiguration WithConverter(SimpleConverter<IEnumerable<string>>? converter)
        {
            _listConverter = converter;
            return this;
        }

        public IIniSerializerConfiguration Apply()
        {
            MappingEntry<IniContext> entry;

            if (_isSingle)
                entry = new SingleIniMapper(_member, _targetType, _converter, _section, _key);
            else
                entry = new ListIniMapper(_member, _targetType, _listConverter, _section, _key);

            _mapper.Entries.Add(entry);
            return _configuration;
        }
    }
}