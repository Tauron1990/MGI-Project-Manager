using System;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal enum MappingType
    {
        Content,
        SingleKey,
        MultiKey
    }

    internal class HeaderedFileKeyCofiguration : IHeaderedFileKeywordConfiguration
    {
        private readonly IHeaderedFileSerializerConfiguration _config;
        private readonly string _keyName;
        private readonly SimpleMapper<HeaderdFileContext> _mapper;
        private readonly MappingType _mappingType;
        private readonly Type _type;
        private SimpleConverter<string>? _converter;

        private string? _member;

        public HeaderedFileKeyCofiguration(IHeaderedFileSerializerConfiguration config,
            SimpleMapper<HeaderdFileContext> mapper, string keyName,
            MappingType mappingType, Type type)
        {
            _config = config;
            _mapper = mapper;
            _keyName = keyName;
            _mappingType = mappingType;
            _type = type;
        }

        public IHeaderedFileSerializerConfiguration Apply()
        {
            _member ??= _keyName;

            MappingEntry<HeaderdFileContext>? map = _mappingType switch
            {
                MappingType.Content => new HeaderedFileContentMapper(_member, _type, _converter),
                MappingType.SingleKey => new HeaderedFileKeyMapper(_member, _type, _converter, _keyName),
                MappingType.MultiKey => new HeaderedFileListKeyMapper(_member, _type, _keyName, _converter),
                _ => null
            };

            if (map != null)
                _mapper.Entries.Add(map);

            return _config;
        }

        public IHeaderedFileKeywordConfiguration WithMember(string name)
        {
            _member = name;
            return this;
        }

        public IHeaderedFileKeywordConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }
    }
}