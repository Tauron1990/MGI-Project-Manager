using System;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText
{
    internal class HeaderedFileContentMapper : MappingEntryBase<HeaderdFileContext>
    {
        private readonly SimpleConverter<string> _converter;

        public HeaderedFileContentMapper([CanBeNull] string membername, [NotNull] Type targetType, [NotNull] SimpleConverter<string> converter)
            : base(membername, targetType)
        {
            _converter = converter;

            if (_converter == null && TargetMember != null)
                _converter = ConverterFactory.CreateConverter(TargetMember, MemberType);
        }

        protected override void Deserialize(object target, HeaderdFileContext context)
        {
            SetValue(target, _converter.ConvertBack(Argument.NotNull(context, nameof(context)).Content));
        }

        protected override void Serialize(object target, HeaderdFileContext context)
        {
            Argument.NotNull(context, nameof(context)).CurrentWriter.Content = _converter.Convert(GetValue(target));
        }
    }
}