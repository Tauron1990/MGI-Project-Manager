using System;
using System.Text.RegularExpressions;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public sealed class PaperFormat
    {
        private static readonly string RegexFormat = "(?<Width>\\d{2}?\\d) cm x (?<Lenght>\\d{2}?\\d) cm";
        public static readonly  string UIFormat    = "[0-9][0-9][05] cm x [0-9][0-9][05] cm";

        private static readonly Regex Regex = new Regex(RegexFormat);

        public PaperFormat(string toMatch)
        {
            if (string.IsNullOrEmpty(toMatch))
            {
                Success = false;
                return;
            }

            var match = Regex.Match(toMatch);

            if (!match.Success) return;

            if (!SetVlaue(match.Groups["Lenght"], li => Lenght = li)) return;

            if (!SetVlaue(match.Groups["Width"], wi => Width = wi)) return;

            Success = true;
        }

        public PaperFormat(int width, int lenght)
        {
            Width  = width;
            Lenght = lenght;

            Success = true;
        }

        public int? Width { get; private set; }

        public int? Lenght { get; private set; }

        public bool Success { get; }

        private bool SetVlaue(Group group, Action<int> setter)
        {
            if (!group.Success) return false;

            if (!int.TryParse(group.Value, out var value)) return false;

            setter(value);

            return true;
        }

        public override string ToString()
        {
            if (Width == null || Lenght == null) return string.Empty;

            return Lenght.Value.ToString("D3") + " cm x " + Width.Value.ToString("D3") + " cm";
        }
    }
}