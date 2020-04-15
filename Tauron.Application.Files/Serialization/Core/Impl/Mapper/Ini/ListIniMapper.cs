using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini
{
    internal sealed class ListIniMapper : MappingEntryBase<IniContext>
    {
        private readonly SimpleConverter<IEnumerable<string>> _converter;
        private readonly string                               _key;
        private readonly string                               _section;

        public ListIniMapper([CanBeNull] string membername, [NotNull] Type targetType, [CanBeNull] SimpleConverter<IEnumerable<string>> converter, [CanBeNull] string section, [CanBeNull] string key)
            : base(membername, targetType)
        {
            _converter = converter;
            _section   = SingleIniMapper.GetSectionForIni(targetType, section);
            _key       = SingleIniMapper.GetKeyForIni(TargetMember, key);

            if (_converter == null && TargetMember != null && MemberType != null)
                _converter = ConverterFactory.CreateListConverter(TargetMember, MemberType);
        }

        protected override void Deserialize(object target, IniContext context)
        {
            Argument.NotNull(context, nameof(context));

            var value = _converter.ConvertBack(context.File.GetSection(_section).GetListEntry(_key).Values);
            SetValue(target, value);
        }

        protected override void Serialize(object target, IniContext context)
        {
            Argument.NotNull(context, nameof(context));

            var value = GetValue(target);
            context.File.GetSection(_section).GetOrAddListEntry(_key).Values.AddRange(_converter.Convert(value));
        }

        public override Exception VerifyError()
        {
            var e = base.VerifyError();

            if (_converter == null)
                e = new SerializerElementNullException("Converter");

            return e ?? _converter.VerifyError();
        }
    }
}