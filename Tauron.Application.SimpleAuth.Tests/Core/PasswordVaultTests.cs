using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.OptionsStore;
using Tauron.Application.SimpleAuth.Core;
using TestHelpers;
using TestHelpers.Options;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.Application.SimpleAuth.Tests.Core
{
    public sealed class PasswordVaultTests
    {
        public enum InternalTestType
        {
            EmptyPassword,
            SamePassword,
            Normal
        }

        private readonly ITestOutputHelper _output;

        public PasswordVaultTests(ITestOutputHelper output) 
            => _output = output;

        [Theory]
        [InlineData("fail")]
        [InlineData("test")]
        [InlineData("admin")]
        public async Task CheckPasswordTest(string pass)
        {
            var hasher = new PasswordHasher<string>();
            var targetPassword = new SimpleOption(PasswordVault.PasswortName, pass != "fail" ? hasher.HashPassword(PasswordVault.PasswortName, pass) : string.Empty, false);

            using var services = HelperCreateDefault.Create<IPasswordVault, PasswordVault>(
                _output,
                configuration: sc =>
                               {
                                   sc.ServiceCollection.Configure<SimplAuthSettings>(s =>
                                                                                     {
                                                                                         s.AppName = "Test-App";
                                                                                         s.BaseAdminPass = "admin";
                                                                                     });

                                   sc.AddService<IOptionsStore, SimpleOptionStore>(() => SimpleOptionStore.CreateSimple(targetPassword));
                               });

            var result = await services.Service.CheckPassword(pass);

            if(pass == "fail")
                Assert.False(result);
            else
                Assert.True(result);
        }

        [Fact]
        public async Task CheckDefaultPassword()
        {
            using var services = HelperCreateDefault.Create<IPasswordVault, PasswordVault>(
                _output,
                configuration: sc =>
                               {
                                   sc.ServiceCollection.Configure<SimplAuthSettings>(s =>
                                                                                     {
                                                                                         s.AppName = "Test-App";
                                                                                         s.BaseAdminPass = "admin";
                                                                                     });

                                   sc.AddService<IOptionsStore, SimpleOptionStore>(SimpleOptionStore.CreateSimple);
                               });

            var result = await services.Service.CheckPassword("admin");

            Assert.True(result);
        }

        [Theory]
        [InlineData("", InternalTestType.EmptyPassword)]
        [InlineData("SamePass", InternalTestType.SamePassword)]
        [InlineData("NormalPass", InternalTestType.Normal)]
        public async Task CheckSetPassword(string pass, InternalTestType testType)
        {
            var option = new SimpleOption(
                PasswordVault.PasswortName, 
                testType == InternalTestType.SamePassword ? new PasswordHasher<string>().HashPassword(PasswordVault.PasswortName, pass) : string.Empty,
                false);

            using var services = HelperCreateDefault.Create<IPasswordVault, PasswordVault>(
                _output,
                configuration: sc =>
                               {
                                   sc.ServiceCollection.Configure<SimplAuthSettings>(s =>
                                                                                     {
                                                                                         s.AppName = "Test-App";
                                                                                         s.BaseAdminPass = "admin";
                                                                                     });

                                   sc.AddService<IOptionsStore, SimpleOptionStore>(
                                       () => SimpleOptionStore.CreateSimple(option),
                                       store =>
                                       {
                                           if (testType == InternalTestType.Normal) Assert.False(string.IsNullOrEmpty(option.Value), "No Passwort was Set");
                                       });
                               });

            var result = await services.Service.SetPassword(pass);

            switch (testType)
            {
                case InternalTestType.EmptyPassword:
                    Assert.False(result, "Empty Test Failed");
                    break;
                case InternalTestType.SamePassword:
                    Assert.False(result, "Same Passwort Test");
                    break;
                case InternalTestType.Normal:
                    Assert.True(result, "Normal Passwort Test");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(testType), testType, null);
            }
        }
    }
}
