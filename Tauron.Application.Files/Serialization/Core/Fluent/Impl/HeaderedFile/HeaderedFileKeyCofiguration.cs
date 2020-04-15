using System;
using JetBrains.Annotations;
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
        private readonly string                               _keyName;
        private readonly SimpleMapper<HeaderdFileContext>     _mapper;
        private readonly MappingType                          _mappingType;
        private readonly Type                                 _type;
        private          SimpleConverter<string>              _converter;

        private string _member;

        public HeaderedFileKeyCofiguration([NotNull] IHeaderedFileSerializerConfiguration config,
                                           [NotNull] SimpleMapper<HeaderdFileContext>     mapper,      [NotNull] string keyName,
                                           MappingType                                    mappingType, [NotNull] Type   type)
        {
            _config      = config;
            _mapper      = mapper;
            _keyName     = keyName;
            _mappingType = mappingType;
            _type        = type;
        }

        public IHeaderedFileSerializerConfiguration Apply()
        {
            if (_member == null) _member = _keyName;

            MappingEntry<HeaderdFileContext> map;

            switch (_mappingType)
            {
                case MappingType.Content:
                    map = new HeaderedFileContentMapper(_member, _type, _converter);
                    break;
                case MappingType.SingleKey:
                    map = new HeaderedFileKeyMapper(_member, _type, _converter, _keyName);
                    break;
                case MappingType.MultiKey:
                    map = new HeaderedFileListKeyMapper(_member, _type, _keyName, _converter);
                    break;
                default:
                    map = null;
                    break;
            }

            if (map == null)
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