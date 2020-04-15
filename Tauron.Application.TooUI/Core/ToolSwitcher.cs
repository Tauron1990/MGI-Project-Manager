using System.Windows.Media.TextFormatting;
using Tauron.Application.ToolUI.ViewModels;

namespace Tauron.Application.ToolUI.Core
{
    public class ToolSwitcher : IToolSwitcher
    {
        private readonly MainWindowViewModel _model;

        public ToolSwitcher(MainWindowViewModel model) 
            => _model = model;

        public void SwitchModel<TType>() where TType : IToolWindow 
            => _model.SwitchModel<TType>();
    }
}