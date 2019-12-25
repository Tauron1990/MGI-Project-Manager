using System.Collections.Generic;
using Catel.Data;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    public sealed class ProcesItem : ObservableObject
    {
        private readonly IList<ProcesItem> _items;

        public string Name { get; }

        public bool Running { get; set; }

        public ProcesItem(string name, IList<ProcesItem> items)
        {
            _items = items;
            Name = name;
            Running = true;
        }

        public ProcesItem Next(string name)
        {
            var newItem = new ProcesItem(name, _items);
            _items.Add(newItem);
            Running = false;

            return newItem;
        }

        public void Finish()
            => Running = false;
    }
}