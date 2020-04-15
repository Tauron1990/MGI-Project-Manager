using System;
using System.Linq;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText
{
    internal class HeaderedFileKeyMapper : MappingEntryBase<HeaderdFileContext>
    {
        private readonly SimpleConverter<string>? _converter;
        private readonly string _keyName;

        public HeaderedFileKeyMapper(string? membername, Type targetType, SimpleConverter<string>? converter, string keyName)
            : base(membername, targetType)
        {
            if (converter == null && TargetMember != null) converter = ConverterFactory.CreateConverter(TargetMember, MemberType);

            _converter = converter;
            _keyName = keyName;
        }

        protected override void Deserialize(object target, HeaderdFileContext context)
        {
            var entry = context.Context[_keyName].First();

            SetValue(target, _converter?.ConvertBack(entry.Content));
        }

        protected override void Serialize(object target, HeaderdFileContext context)
        {
            var content = _converter?.Convert(GetValue(target));

            Argument.NotNull(context, nameof(context)).CurrentWriter.Add(_keyName, content ?? string.Empty);
        }

        public override Exception? VerifyError()
        {
            var e = base.VerifyError();
            if (_keyName == null)
                e = new SerializerElementNullException("KeyName");
            if (_converter == null)
                e = new SerializerElementNullException("Coverter");

            return e;
        }
    }
}