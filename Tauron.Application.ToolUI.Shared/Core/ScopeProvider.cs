using System;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.Application.ToolUI.Core
{
    public abstract class ScopeProvider : ViewModelBase
    {
        public IServiceScope? Scope { get; set; }

        protected IServiceProvider ServiceProvider => Argument.NotNull(Scope, nameof(Scope)).ServiceProvider;
    }
}