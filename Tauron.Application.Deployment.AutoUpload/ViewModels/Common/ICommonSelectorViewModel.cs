using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    public interface ICommonSelectorViewModel
    {
        event Action<SelectorItemBase?>? ElementSelected;

        void Init(IEnumerable<SelectorItemBase> items, bool addNew, Func<SelectorItemBase, Task> selectedItemAction);

        Task Run();

        bool CanRun();
    }
}