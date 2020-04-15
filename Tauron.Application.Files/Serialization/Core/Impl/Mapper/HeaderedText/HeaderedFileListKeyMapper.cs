using System;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText
{
    internal class HeaderedFileListKeyMapper : MappingEntryBase<HeaderdFileContext>
    {
        private readonly SimpleConverter<string> _converter;
        private readonly string                  _keyName;

        private readonly ListBuilder _listBuilder;

        public HeaderedFileListKeyMapper([CanBeNull] string membername, [NotNull] Type targetType, [CanBeNull] string keyName, [CanBeNull] SimpleConverter<string> converter)
            : base(membername, targetType)
        {
            _keyName   = keyName;
            _converter = converter;

            if (TargetMember == null) return;

            _listBuilder = new ListBuilder(MemberType);

            var elementType = _listBuilder.ElemenType;

            if (_converter == null && elementType != null) _converter = ConverterFactory.CreateConverter(TargetMember, elementType);
        }

        protected override void Deserialize(object target, HeaderdFileContext context)
        {
            Argument.NotNull(context, nameof(context));

            _listBuilder.Begin(null, false);

            try
            {
                foreach (var contextEnry in context.Context[_keyName])
                {
                    _listBuilder.Add(_converter.ConvertBack(contextEnry.Content));
                }
            }
            finally
            {
                SetValue(target, _listBuilder.End());
            }
        }

        protected override void Serialize(object target, HeaderdFileContext context)
        {
            Argument.NotNull(context, nameof(context));

            _listBuilder.Begin(GetValue(target), true);

            try
            {
                var writer = context.CurrentWriter;

                foreach (var obj in _listBuilder.Objects)
                {
                    writer.Add(_keyName, _converter.Convert(obj));
                }
            }
            finally
            {
                _listBuilder.End();
            }
        }
    }
}