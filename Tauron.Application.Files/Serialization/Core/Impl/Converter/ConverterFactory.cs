using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal static class ConverterFactory
    {
        public static SimpleConverter<string> CreateConverter(MemberInfo? member, Type? targetType)
        {
            if (member == null || targetType == null) return new InvalidConverter();

            if (targetType == typeof(string)) return new StringCnverter();
            if (targetType.BaseType == typeof(Enum)) return new GenericEnumConverter(targetType);

            return new TypeConverterConverter(GetConverter(member, targetType));
        }

        public static SimpleConverter<IEnumerable<string>> CreateListConverter([CanBeNull] MemberInfo member, [CanBeNull] Type targeType)
        {
            if (member == null || targeType == null) return new InvalidEnumConverter();

            var builder = new ListBuilder(targeType);

            var converter = CreateConverter(member, builder.ElemenType);

            return new UniversalListConverter(converter, builder);
        }

        private static TypeConverter GetConverter([NotNull] MemberInfo info, [NotNull] Type memberType)
        {
            var targetType = memberType;

            var attr = info.GetCustomAttributes<TypeConverterAttribute>().FirstOrDefault();
            if (attr == null) return TypeDescriptor.GetConverter(targetType);

            var target = GetTypeFromName(attr.ConverterTypeName, targetType);
            if (target == null) return TypeDescriptor.GetConverter(targetType);

            var converter = Activator.CreateInstance(target) as TypeConverter;
            return converter ?? TypeDescriptor.GetConverter(targetType);
        }

        private static Type? GetTypeFromName([NotNull] string typeName, [CanBeNull] Type memberType)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            var num = typeName.IndexOf(',');
            Type? type = null;
            if (num == -1 && memberType != null) type = memberType.Assembly.GetType(typeName);
            if (type == null) type = Type.GetType(typeName);
            if (type == null && num != -1) type = Type.GetType(typeName.Substring(0, num));
            return type;
        }

        private class InvalidConverter : SimpleConverter<string>
        {
            public override object ConvertBack(string target)
            {
                throw new NotSupportedException();
            }

            public override string Convert(object? source)
            {
                throw new NotSupportedException();
            }

            public override Exception VerifyError()
            {
                return new ArgumentException("Member or Target Type Was null");
            }
        }

        private class InvalidEnumConverter : SimpleConverter<IEnumerable<string>>
        {
            public override object ConvertBack(IEnumerable<string> target)
            {
                throw new NotSupportedException();
            }

            public override IEnumerable<string> Convert(object? source)
            {
                throw new NotSupportedException();
            }

            public override Exception VerifyError()
            {
                return new ArgumentException("Member or Target Type Was null");
            }
        }
    }
}