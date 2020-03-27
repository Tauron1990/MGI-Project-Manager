using System;
using Microsoft.Extensions.Options;
using Tauron.Application.SimpleAuth.Core;
using Xunit;

namespace Tauron.Application.SimpleAuth.Tests.Core
{
    public sealed class SimpleAuthenticationPostConfigureOptionsTest
    {
        [Fact]
        public void PostConfigureTest()
        {
            const string testName = "Test";

            var options = new SimplAuthSettings();
            var autOptions = new SimpleAuthenticationOptions();

            var test = new SimpleAuthenticationPostConfigureOptions(new OptionsWrapper<SimplAuthSettings>(options));


            //Valid 
            autOptions.Realm = "Test";
            options.AppName = "Test";
            autOptions.TokenTimeout = TimeSpan.FromDays(1);

            test.PostConfigure(testName, autOptions);

            //PatiallyValid
            autOptions.Realm = "";

            test.PostConfigure(testName, autOptions);

            //Invalid 1
            autOptions.Realm = "";
            options.AppName = "";

            Assert.Throws<InvalidOperationException>(() => test.PostConfigure(testName, autOptions));

            //Invalid 2
            autOptions.Realm = "Test";
            autOptions.TokenTimeout = TimeSpan.FromMilliseconds(1000);

            Assert.Throws<InvalidOperationException>(() => test.PostConfigure(testName, autOptions));
        }
    }
}