using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;

namespace Tauron.Application.Deployment.AutoUpload.Views.Common
{
    /// <summary>
    ///     Interaktionslogik für CommonSelectorView.xaml
    /// </summary>
    public partial class CommonSelectorView : ICommonSelectorViewModel
    {
        private readonly CommonSelectorViewModel _model;

        public CommonSelectorView(CommonSelectorViewModel model)
            : base(model)
        {
            _model = model;
            InitializeComponent();
        }


        public event Action<SelectorItemBase?>? ElementSelected
        {
            add => _model.ElementSelected += value;
            remove => _model.ElementSelected -= value;
        }

        public void Init(IEnumerable<SelectorItemBase> items, bool addNew, Func<SelectorItemBase, Task> selectedItemAction)
        {
            _model.Init(items, addNew, selectedItemAction);
        }

        public Task Run()
        {
            return _model.Run();
        }

        public bool CanRun()
        {
            return _model.CanRun();
        }
    }
}