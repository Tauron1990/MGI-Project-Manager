namespace Tauron.Application.Models.Rules
{
    public sealed class RequiredRule : ModelRule
    {
        public RequiredRule()
        {
            Id      = "RequiredRule";
            Message = () => ResourceMessages.RequireRuleError.SFormat(FieldName);
        }

        public string FieldName { get; set; }

        public bool AllowStringEmpty { get; set; }

        public override bool IsValidValue(object obj, ValidatorContext context)
        {
            if (obj == null) return false;
            var str = obj as string;

            if (str == null || AllowStringEmpty) return true;

            return !string.IsNullOrWhiteSpace(str);
        }
    }
}