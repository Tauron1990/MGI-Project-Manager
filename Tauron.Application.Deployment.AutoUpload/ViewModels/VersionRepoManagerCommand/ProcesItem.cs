using Catel.Data;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    public sealed class ProcesItem : ObservableObject
    {
        public string Name { get; }

        public bool Running { get; set; }

        public ProcesItem(string name)
        {
            Name = name;
            Running = true;
        }
    }
}