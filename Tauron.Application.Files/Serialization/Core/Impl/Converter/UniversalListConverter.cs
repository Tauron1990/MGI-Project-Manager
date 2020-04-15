using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class UniversalListConverter : SimpleConverter<IEnumerable<string>>
    {
        private readonly SimpleConverter<string> _baseConverter;
        private readonly ListBuilder _listBuilder;

        public UniversalListConverter([NotNull] SimpleConverter<string> baseConverter, [NotNull] ListBuilder listBuilder)
        {
            _baseConverter = Argument.NotNull(baseConverter, nameof(baseConverter));
            _listBuilder = Argument.NotNull(listBuilder, nameof(listBuilder));
        }

        public override object ConvertBack([NotNull] IEnumerable<string> target)
        {
            _listBuilder.Begin(null, false);

            foreach (var content in target) _listBuilder.Add(_baseConverter.ConvertBack(content));

            return Argument.CheckResult(_listBuilder.End(), "List Builder Result");
        }

        public override IEnumerable<string> Convert(object? source)
        {
            _listBuilder.Begin(source, true);

            var temp = (from object value in _listBuilder.Objects select _baseConverter.Convert(value)).ToArray();

            _listBuilder.End();

            return temp;
        }
    }
}