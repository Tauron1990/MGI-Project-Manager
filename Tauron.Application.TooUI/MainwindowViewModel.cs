using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Deployment.AutoUpload.ViewModels;
using Tauron.Application.Logging;
using Tauron.Application.ToolUI.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.ToolUI
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly ConcurrentDictionary<Type, ObjectFactory> _logContexts = new ConcurrentDictionary<Type, ObjectFactory>();

        private readonly IWpfLifetime _lifetime;
        private readonly IServiceScopeFactory _serviceProvider;
        private readonly ISLogger<MainWindowViewModel> _logger;
        private IServiceScope? _currentScope;

        public MainWindowViewModel(IWpfLifetime lifetime, IServiceScopeFactory serviceProvider, ISLogger<MainWindowViewModel> logger)
        {
            _lifetime = lifetime;
            _serviceProvider = serviceProvider;
            _logger = logger;
            AlwaysInvokeNotifyChanged = true;
        }

        protected override Task InitializeAsync()
        {
            CloseTool();

            return base.InitializeAsync();
        }

        public IToolWindow? MainContent { get; set; }

        [CommandTarget]
        public void StartUpload()
        {
            _logger.Information("Open Upload Tool");
            SwitchModel<UploadToolWindowViewModel>();
        }

        [CommandTarget]
        public void CloseTool()
        {
            _logger.Information("Colsing Current Tool");
            SwitchModel<ToolSelectViewModel>();
        }

        [CommandTarget]
        public void CloseApp()
        {
            _logger.Information("Application Shutdown");
            _lifetime.Shutdown();
        }

        internal void SwitchModel<TType>()
            where TType : IToolWindow
        {
            Task.Run(() =>
            {
                _logger.Information("Switch Model {ModelType}", typeof(TType));

                MainContent?.CloseViewModelAsync(false);
                _currentScope?.Dispose();

                _currentScope = _serviceProvider.CreateScope();
                var attr = typeof(TType).GetCustomAttribute<LogContextProviderAttribute>();
                if (attr != null)
                {
                    var factory = _logContexts.GetOrAdd(attr.ContextType, t => ActivatorUtilities.CreateFactory(t, Array.Empty<Type>()));

                    if (factory(_currentScope.ServiceProvider, Array.Empty<object>()) is LogContextBase context)
                        context.Apply();
                    else
                        _logger.Warning("Context Type not Compatiple {ModelType}", typeof(TType));

                }
                else
                    _logger.Warning("No LogContext for Tool Provided {ModelType}", typeof(TType));

                MainContent = _currentScope.ServiceProvider.GetRequiredService<TType>();
            });
        }
    }
}