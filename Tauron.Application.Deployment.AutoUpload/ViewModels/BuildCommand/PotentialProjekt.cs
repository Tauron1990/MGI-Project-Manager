using System;
using System.Threading.Tasks;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    public sealed class PotentialProjekt
    {
        public readonly Func<Task> Action; 

        public string Name { get; }

        public PotentialProjekt(string name, Func<Task> action)
        {
            Name = name;
            Action = action;
        }
    }
}