using Tauron.Application.Models;

namespace Tauron.Application
{
    public abstract class TabWorkspaceHolderBase<TWorkspace> : ViewModelBase, IWorkspaceHolder
        where TWorkspace : class, ITabWorkspace
    {
        protected TabWorkspaceHolderBase()
        {
            Tabs = new WorkspaceManager<TWorkspace>(this);
        }

        public WorkspaceManager<TWorkspace> Tabs { get; protected set; }

        public virtual void Register(ITabWorkspace workspace)
        {
        }

        public virtual void UnRegister(ITabWorkspace workspace)
        {
        }
    }
}