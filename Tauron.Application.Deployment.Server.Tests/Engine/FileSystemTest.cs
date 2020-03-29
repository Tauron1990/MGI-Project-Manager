using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Deployment.Server.Engine;
using Tauron.Application.Deployment.Server.Engine.Impl;
using TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.Application.Deployment.Server.Tests.Engine
{
    public sealed class FileSystemTest
    {
        private readonly ITestOutputHelper _helper;

        public FileSystemTest(ITestOutputHelper helper)
            => _helper = helper;

        [Theory]
        [InlineData(ServerFileMode.ContentRoot, "Run")]
        [InlineData(ServerFileMode.ApplicationData, "")]
        public void TestRepositoryPath(ServerFileMode mode, string root)
        {
            var test = ServiceTest
               .Create<IFileSystem, FileSystem>(_helper,
                    config: sc =>
                            {
                                sc.Switch<ServerFileMode>()
                                   .Case(ServerFileMode.ContentRoot,
                                        c =>
                                        {
                                            c.AddMock<IWebHostEnvironment>().With(m => m.SetupGet(p => p.ContentRootPath).Returns(root))
                                               .AddService();
                                        })
                                   .Case(ServerFileMode.ApplicationData, c => c.AddMock<IWebHostEnvironment>())
                                   .Apply(mode);

                                sc.ServiceCollection.Configure<LocalSettings>(l => l.ServerFileMode = mode);
                            });

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            test.Run(service =>
                     {
                         Assert.False(string.IsNullOrEmpty(service.RepositoryRoot), "Repository Root is Empty");

                         switch (mode)
                         {
                             case ServerFileMode.ApplicationData:
                                Assert.StartsWith(appData, service.RepositoryRoot, StringComparison.Ordinal);
                                 break;
                             case ServerFileMode.ContentRoot:
                                 Assert.StartsWith(root, service.RepositoryRoot, StringComparison.Ordinal);
                                 break;
                             case ServerFileMode.Unkowen:
                                 Assert.False(false, "Wrong Mode");
                                 break;
                             default:
                                 throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                         }
                     });
        }
    }
}