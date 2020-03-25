using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.SimpleAuth.Core;
using Tauron.Application.SimpleAuth.Tests.Mocks;
using TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.Application.SimpleAuth.Tests.Core
{
    public class TokenManagerTest
    {
        public enum TestType
        {
            Match,
            Mismatch,
            Outdate,
            Empty,
        }

        private readonly ITestOutputHelper _testOutputHelper;

        public TokenManagerTest(ITestOutputHelper testOutputHelper) 
            => _testOutputHelper = testOutputHelper;


        [Theory]
        [InlineData("Test1", TestType.Match)]
        [InlineData("Test2", TestType.Outdate)]
        [InlineData("Test3", TestType.Mismatch)]
        [InlineData("", TestType.Empty)]
        public void TokenGenerateTest(string realm, TestType type)
        {
            var options = new SimpleAuthenticationOptions();
            var clock = new MockSystemClock();

            using var service = HelperCreateDefault.Create<ITokenManager, TokenManager>(_testOutputHelper,
                configuration: sc =>
                {
                    sc.ServiceCollection.Configure<SimpleAuthenticationOptions>(ao =>
                    {
                        ao.Realm = realm;
                        options = ao;
                    });

                    sc.AddService<ISystemClock, MockSystemClock>(() => clock);
                });

            switch (type)
            {
                case TestType.Match:
                    var result1 = service.Service.ValidateToken(service.Service.GenerateToken());
                    Assert.True(result1, "No Token Match");
                    break;
                case TestType.Mismatch:
                    var token = service.Service.GenerateToken();
                    options.Realm = "Hallo";
                    var result2 = service.Service.ValidateToken(token);

                    Assert.False(result2, "Token should be Realm mismatch");
                    break;
                case TestType.Outdate:
                    var token2 = service.Service.GenerateToken();
                    clock.TargetDate = DateTimeOffset.UtcNow.AddDays(2);
                    var result3 = service.Service.ValidateToken(token2);

                    Assert.False(result3, "Token should be Outdated");
                    break;
                case TestType.Empty:
                    var result4 = service.Service.ValidateToken(string.Empty);

                    Assert.False(result4, "Empty Token Validate not False");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}