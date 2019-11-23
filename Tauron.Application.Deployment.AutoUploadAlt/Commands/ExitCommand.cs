using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tauron.Application.Deployment.AutoUpload.Build;

namespace Tauron.Application.Deployment.AutoUpload.Commands
{
    [Command("exit", "Beendet das Programm")]
    public class ExitCommand : CommandBase
    {
        public override Task<bool> Execute(ApplicationContext context, ConsoleUi ui, InputManager input) 
            => Task.FromResult(false);
    }
}
