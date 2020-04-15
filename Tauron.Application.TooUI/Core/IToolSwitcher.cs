using Tauron.Application.ToolUI.ViewModels;

namespace Tauron.Application.ToolUI.Core
{
    public interface IToolSwitcher
    {
        void SwitchModel<TType>()
            where TType : IToolWindow;
    }
}