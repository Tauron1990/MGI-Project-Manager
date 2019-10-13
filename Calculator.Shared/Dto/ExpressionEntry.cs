namespace Calculator.Shared.Dto
{
    public class ExpressionEntry
    {
        public string Expression { get; set; }

        public string Result { get; set; }

        public ExpressionEntry()
        {
            
        }

        public ExpressionEntry(string expression, string result)
        {
            Expression = expression;
            Result = result;
        }
    }
}