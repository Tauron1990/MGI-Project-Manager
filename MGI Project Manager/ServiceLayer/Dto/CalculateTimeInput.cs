namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public sealed class CalculateTimeInput
    {
        public CalculateTimeInput(long? iterations, PaperFormat paperFormat, long? amount, double? speed)
        {
            Iterations  = iterations;
            PaperFormat = paperFormat;
            Amount      = amount;
            Speed       = speed;
        }

        public long?       Iterations  { get; }
        public PaperFormat PaperFormat { get; }
        public long?       Amount      { get; }
        public double?     Speed       { get; }
    }
}