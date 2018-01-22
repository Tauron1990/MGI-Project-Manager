namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public class SaveOutput
    {
        public SaveOutput(bool succsess, string message)
        {
            Succsess = succsess;
            Message  = message;
        }

        public bool   Succsess { get; }
        public string Message  { get; }
    }
}