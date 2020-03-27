using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tauron.Application.SimpleAuth.Api;
using Tauron.Application.SimpleAuth.Core;
using Tauron.Application.SimpleAuth.Data;
using TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.Application.SimpleAuth.Tests.Api
{
    public sealed class LoginControllerTest
    {
        public LoginControllerTest(ITestOutputHelper helper)
            => _helper = helper;

        private readonly ITestOutputHelper _helper;

        private void Init(ControllerBase controller)
        {
            //var col = new FeatureCollection();
            //col.Set(new HttpConnectionFeature { ConnectionId = "Test"});
            var cc = new ControllerContext {HttpContext = new DefaultHttpContext()};
            cc.HttpContext.Features.Set(new HttpConnectionFeature {ConnectionId = "Test-Id"});

            controller.ControllerContext = cc;
        }

        [Fact]
        public void TokenGenerationExceptionTest()
        {
            var con =
                ServiceTest
                   .Create<LoginV1Controller>(_helper,
                        config: sc =>
                                {
                                    sc.AddMock<IPasswordVault>().AddService()
                                       .AddMock<ITokenManager>()
                                       .With(m
                                                => m.Setup(tm => tm.GenerateToken()).Throws<InvalidOperationException>())
                                       .Assert(m => m.Verify(tm => tm.GenerateToken(), Times.Exactly(1)))
                                       .AddService();
                                });

            con.Test(c =>
                     {
                         Init(c);

                         var token = c.GetToken();

                         Assert.False(token.Successful);
                         Assert.NotEqual(string.Empty, token.FailMessage);
                     });
        }

        [Fact]
        public void TokenGenerationOkTest()
        {
            const string testToken = nameof(testToken);

            var con =
                ServiceTest
                   .Create<LoginV1Controller>(_helper,
                        config: sc =>
                                {
                                    sc.AddMock<IPasswordVault>().AddService()
                                       .AddMock<ITokenManager>()
                                       .With(m
                                                => m.Setup(tm => tm.GenerateToken()).Returns(testToken))
                                       .Assert(m => m.Verify(tm => tm.GenerateToken(), Times.Exactly(1)))
                                       .AddService();
                                });

            con.Test(c =>
                     {
                         Init(c);

                         var token = c.GetToken();

                         Assert.NotNull(token);
                         Assert.True(token.Successful);
                         Assert.Equal(testToken, token.Token);
                     });
        }

        public enum PasswordTest
        {
            Valid,
            Exception,
            Fail,
            Wrong,
        }

        [Theory]
        [InlineData("Test", PasswordTest.Valid)]
        [InlineData("Test2", PasswordTest.Wrong)]
        [InlineData("Test3", PasswordTest.Exception)]
        [InlineData("Test4", PasswordTest.Fail)]
        public async Task SetPasswordTest(string pass, PasswordTest testType)
        {
            const string exceptionMessage = "Test Fehler";

            var can =
                ServiceTest
                   .Create<LoginV1Controller>(_helper,
                        config: sc =>
                                {
                                    sc.AddMock<ITokenManager>().With(m => m.Setup(tm => tm.GenerateToken()).Returns("TestToken")).AddService();

                                    sc.Switch<PasswordTest>()
                                       .Case(PasswordTest.Fail,
                                            c =>
                                            {
                                                c.AddMock<IPasswordVault>()
                                                   .With(m => m.Setup(pv => pv.CheckPassword(It.IsIn(pass))).ReturnsAsync(true))
                                                   .With(m => m.Setup(pv => pv.SetPassword(It.IsIn(pass))).ReturnsAsync(false))
                                                   .AddService();
                                            })
                                       .Case(PasswordTest.Exception,
                                            c =>
                                            {
                                                c.AddMock<IPasswordVault>()
                                                   .With(m => m.Setup(pv => pv.CheckPassword(It.IsAny<string>())).ThrowsAsync(new Exception(exceptionMessage)))
                                                   .AddService();
                                            })
                                       .Case(PasswordTest.Wrong,
                                            c =>
                                            {
                                                c.AddMock<IPasswordVault>()
                                                   .With(m => m.Setup(pv => pv.CheckPassword(It.IsAny<string>())).ReturnsAsync(false))
                                                   .AddService();
                                            })
                                       .Default(c =>
                                                {
                                                    c.AddMock<IPasswordVault>()
                                                       .With(m => m
                                                               .Setup(pv => pv.CheckPassword(It.IsAny<string>()))
                                                               .ReturnsAsync(true))
                                                       .With(m => m
                                                               .Setup(pv => pv.SetPassword(It.IsIn(pass)))
                                                               .ReturnsAsync(true))
                                                       .AddService();
                                                })
                                       .Apply(testType);
                                });

            await can.Test(async service =>
                           {
                               Init(service);

                               var result = await service.Setpassword(new NewPasswordData
                                                                {
                                                                    NewPassword = pass,
                                                                    OldPassword = pass
                                                                });

                               switch (testType)
                               {
                                   case PasswordTest.Valid:
                                       Assert.NotEqual(string.Empty, result.Token);
                                       Assert.True(result.Successful, "Password set Not Successful");
                                       break;
                                   case PasswordTest.Exception:
                                       Assert.False(result.Successful, "Password Set should be False");
                                       Assert.Equal(exceptionMessage, result.FailMessage);
                                       break;
                                   case PasswordTest.Fail:
                                   case PasswordTest.Wrong:
                                       Assert.NotEqual(string.Empty, result.FailMessage);
                                       Assert.False(result.Successful, "Password Set should be False");
                                       break;
                                   default:
                                       throw new ArgumentOutOfRangeException(nameof(testType), testType, null);
                               }
                           });
        }
    }
}