using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Tauron.Application.Wpf.AppCore
{
    public abstract class WpfStartup : StartupBase<System.Windows.Application, IHostEnvironment>
    {
        protected WpfStartup([NotNull] IConfiguration configuration) : base(configuration)
        {
        }
    }
}