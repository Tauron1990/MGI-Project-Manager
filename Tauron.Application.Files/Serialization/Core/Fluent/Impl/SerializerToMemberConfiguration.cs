using System;
using System.IO;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent.Impl
{
    internal class SerializerToMemberConfiguration<TTInterface, TMapperContext> : ISerializerToMemberConfiguration<TTInterface>
        where TMapperContext : IOrginalContextProvider
        where TTInterface : class, ISerializerRootConfiguration
    {
        private readonly SimpleMapper<TMapperContext> _mapper;
        private readonly Type _serialType;
        private readonly TTInterface _targetConfiguration;
        private readonly string _targetMember;
        private Func<object, SerializerMode, Stream>? _open;
        private Func<string?, IStreamSource>? _relativeFunc;

        private ISerializer? _serializer;

        public SerializerToMemberConfiguration(string targetMember, TTInterface targetConfiguration, SimpleMapper<TMapperContext> mapper,
            Type serialType)
        {
            _targetMember = targetMember;
            _targetConfiguration = targetConfiguration;
            _mapper = mapper;
            _serialType = serialType;
        }

        public ISerializerToMemberConfiguration<TTInterface> WithSourceSelector(Func<object, SerializerMode, Stream> open, Func<string?, IStreamSource>? relativeFunc)
        {
            _open = open;
            _relativeFunc = relativeFunc;
            return this;
        }

        public ISerializerToMemberConfiguration<TTInterface> WithSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public TTInterface Apply()
        {
            if (_open == null)
                throw new InvalidOperationException("Open Func Null");

            _mapper.Entries.Add(new SerializerBindMapper<TMapperContext>(_targetMember, _serialType, _serializer,
                new SerializerBindMapper<TMapperContext>.StreamManager(_open, _relativeFunc)));
            return _targetConfiguration;
        }
    }
}