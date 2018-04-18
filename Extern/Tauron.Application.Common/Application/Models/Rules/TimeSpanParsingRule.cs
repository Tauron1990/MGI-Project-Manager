using System;
using System.Globalization;

namespace Tauron.Application.Models.Rules
{
    public sealed class TimeSpanParsingRule : ModelRule
    {
        private readonly bool   _mustBePositive;
        private          string _message;

        public TimeSpanParsingRule(bool mustBePositive = true)
        {
            _mustBePositive = mustBePositive;
            Message         = () => _message;
        }

        public override bool IsValidValue(object obj, ValidatorContext context)
        {
            try
            {
                var span = TimeSpan.Parse((string) obj, CultureInfo.CurrentUICulture);
                if (!_mustBePositive && span.Ticks >= 0) return true;

                TimeSpan.Parse("-100000000000000000000000000");
                return true;
            }
            catch (Exception e) when (e is FormatException || e is OverflowException || e is ArgumentException)
            {
                _message = e.Message;
                return false;
            }
        }
    }
}