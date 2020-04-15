using System;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText
{
    internal class HeaderedFileListKeyMapper : MappingEntryBase<HeaderdFileContext>
    {
        private readonly SimpleConverter<string>? _converter;
        private readonly string? _keyName;

        private readonly ListBuilder? _listBuilder;

        public HeaderedFileListKeyMapper(string? membername, Type targetType, string? keyName, SimpleConverter<string>? converter)
            : base(membername, targetType)
        {
            _keyName = keyName;
            _converter = converter;

            if (TargetMember == null) return;

            _listBuilder = new ListBuilder(MemberType);

            var elementType = _listBuilder.ElemenType;

            if (_converter == null && elementType != null) _converter = ConverterFactory.CreateConverter(TargetMember, elementType);
        }

        protected override void Deserialize(object target, HeaderdFileContext context)
        {
            Argument.NotNull(context, nameof(context));

            if (string.IsNullOrEmpty(_keyName)) return;

            _listBuilder?.Begin(null, false);

            try
            {
                foreach (var contextEnry in context.Context[_keyName]) _listBuilder?.Add(_converter?.ConvertBack(contextEnry.Content));
            }
            finally
            {
                SetValue(target, _listBuilder?.End());
            }
        }

        protected override void Serialize(object target, HeaderdFileContext context)
        {
            Argument.NotNull(context, nameof(context));

            if (string.IsNullOrEmpty(_keyName)) return;
            _listBuilder?.Begin(GetValue(target), true);

            try
            {
                var writer = context.CurrentWriter;

                foreach (var obj in _listBuilder?.Objects ?? Array.Empty<object>())
                {
                    var cobj = _converter?.Convert(obj);
                    if (cobj == null) continue;
                    writer.Add(_keyName, cobj);
                }
            }
            finally
            {
                _listBuilder?.End();
            }
        }
    }
}