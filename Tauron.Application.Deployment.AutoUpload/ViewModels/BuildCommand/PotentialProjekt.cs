using System;
using System.Threading.Tasks;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    public sealed class PotentialProjekt
    {
        private readonly Func<Task> _action; 

        public string Name { get; }

        public PotentialProjekt(string name, Func<Task> action)
        {
            Name = name;
            _action = action;
        }
    }
}