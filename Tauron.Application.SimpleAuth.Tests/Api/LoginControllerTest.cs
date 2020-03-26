using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tauron.Application.SimpleAuth.Api;
using Tauron.Application.SimpleAuth.Core;
using TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.Application.SimpleAuth.Tests.Api
{
    public sealed class LoginControllerTest
    {
        private readonly ITestOutputHelper _helper;

        public LoginControllerTest(ITestOutputHelper helper) 
            => _helper = helper;

        private void Init(ControllerBase controller)
        {
            //var col = new FeatureCollection();
            //col.Set(new HttpConnectionFeature { ConnectionId = "Test"});
            var cc = new ControllerContext { HttpContext = new DefaultHttpContext()};
            cc.HttpContext.Features.Set(new HttpConnectionFeature {ConnectionId = "Test-Id"});

            controller.ControllerContext = cc;
        }

        [Fact]
        public void TokenGenerationOkTest()
        {
            const string TestToken = nameof(TestToken);

            using var con = HelperCreateDefault.Create<LoginV1Controller>(_helper, 
                configuration: sc =>
                {
                    sc.AddMock<IPasswordVault>().BuildService()
                        .AddMock<ITokenManager>().For(m =>
                        {
                            m.Setup(tm => tm.GenerateToken()).Returns(TestToken);
                        }).WithAssert(m => m.Verify(tm => tm.GenerateToken(), Times.Exactly(1))).BuildService();
                });

            Init(con.Service);

            var token = con.Service.GetToken();

            Assert.NotNull(token);
            Assert.True(token.Successful);
            Assert.Equal(TestToken, token.Token);
        }
    }
}
