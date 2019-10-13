
using CQRSlite.Commands;

namespace Calculator.Shared.Commands
{
    public class RunExpressionCommand : ICommand
    {
        public string Input { get; set; }
    }
}