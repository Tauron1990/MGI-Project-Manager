namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public sealed class CalculateValidateOutput
    {
        public CalculateValidateOutput(bool valid, string message)
        {
            Valid   = valid;
            Message = message;
        }

        public bool   Valid   { get; }
        public string Message { get; }
    }
}