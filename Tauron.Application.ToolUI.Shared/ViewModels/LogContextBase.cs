using System;
using Serilog.Context;

namespace Tauron.Application.ToolUI.ViewModels
{
    public abstract class LogContextBase : IDisposable
    {
        public const string ToolType = nameof(ToolType);

        private IDisposable? _entry;

        public void Apply() => _entry = LogContext.PushProperty(ToolType, ContextData());

        protected abstract object ContextData();

        public void Dispose() => _entry?.Dispose();
    }
}