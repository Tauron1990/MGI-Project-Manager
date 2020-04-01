using Microsoft.AspNetCore.DataProtection.Repositories;
using TestHelpers;
using Xunit.Abstractions;

namespace Tauron.Application.Deployment.Server.Tests.Engine.Provider
{
    public sealed class RepositoryManagerTests
    {
        private readonly ITestOutputHelper _helper;

        public RepositoryManagerTests(ITestOutputHelper helper) 
            => _helper = helper;

        private TestService<IXmlRepository>
    }
}