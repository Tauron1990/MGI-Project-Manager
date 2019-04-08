using Microsoft.AspNetCore.Hosting;
using Tauron.Application.MgiProjectManager.Server.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace Tauron.Application.MgiProjectManager.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}