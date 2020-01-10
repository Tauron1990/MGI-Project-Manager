using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.MVVM;
using Tauron.Application.Deployment.AutoUpload.Views.Common;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    public sealed class CommonSelectorViewModel : ViewModelBase, ICommonSelectorViewModel
    {
        private Func<SelectorItemBase, Task>? _runner;

        public FastObservableCollection<SelectorItemBase> Items { get; } = new FastObservableCollection<SelectorItemBase>();

        public SelectorItemBase? SelectedItem { get; set; }

        private CommonSelectorViewModel()
        {

        }

        public static ICommonSelectorViewModel Create()
        {
            if (System.Windows.Application.Current.Dispatcher != null) 
                return System.Windows.Application.Current.Dispatcher.Invoke(() => new CommonSelectorView(new CommonSelectorViewModel()));
            throw new InvalidOperationException();
        }

        public event Action<SelectorItemBase?>? ElementSelected;

        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SelectedItem))
                ElementSelected?.Invoke(SelectedItem);
            base.OnModelPropertyChanged(sender, e);
        }

        public void Init(IEnumerable<SelectorItemBase> items, bool addNew, Func<SelectorItemBase, Task> selectedItemAction)
        {
            Items.Clear();
            Items.AddItems(items);

            if(addNew)
                Items.Add(new NewSelectorItem("<Neu>"));

            _runner = selectedItemAction;
        }

        public async Task Run()
        {
            if (SelectedItem == null || _runner == null) return;

            await _runner(SelectedItem);
        }

        public bool CanRun() 
            => SelectedItem != null;
    }
}