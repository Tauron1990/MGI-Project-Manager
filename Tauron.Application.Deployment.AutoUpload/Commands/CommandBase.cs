using System.Threading.Tasks;
using Tauron.Application.Deployment.AutoUpload.Build;

namespace Tauron.Application.Deployment.AutoUpload.Commands
{
    public abstract class CommandBase
    {
        public virtual void PrintHeader(ConsoleUi ui)
        {

        }

        public abstract Task<bool> Execute(ApplicationContext context, ConsoleUi ui, InputManager input);
    }
}