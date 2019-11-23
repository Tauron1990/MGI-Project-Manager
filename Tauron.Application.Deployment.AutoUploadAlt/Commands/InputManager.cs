using System.Threading.Tasks;

namespace Tauron.Application.Deployment.AutoUpload.Commands
{
    public abstract class InputManager
    {
        public abstract Task<string> ReadLine(string description);
    }
}