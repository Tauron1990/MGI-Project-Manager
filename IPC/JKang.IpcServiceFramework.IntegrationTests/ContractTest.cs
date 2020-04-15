using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JKang.IpcServiceFramework.IntegrationTests
{
    public class ContractTest : IDisposable
    {
        public ContractTest()
        {
            // configure DI
            var services = new ServiceCollection()
               .AddIpc(builder => builder.AddNamedPipe().AddService<ITestService, TestService>());
            _port = new Random().Next(10000, 50000);
            var host = new IpcServiceHostBuilder(services.BuildServiceProvider())
               .AddTcpEndpoint<ITestService>(
                    Guid.NewGuid().ToString(),
                    IPAddress.Loopback,
                    _port)
               .Build();
            _cancellationToken = new CancellationTokenSource();
            host.RunAsync(_cancellationToken.Token);

            _client = new IpcServiceClientBuilder<ITestService>()
               .UseTcp(IPAddress.Loopback, _port)
               .Build();
        }

        public void Dispose()
        {
            _cancellationToken.Cancel();
        }

        private readonly CancellationTokenSource _cancellationToken;
        private readonly int _port;
        private readonly IpcServiceClient<ITestService> _client;

        [Theory]
        [AutoData]
        public async Task SimpleType(float a, float b)
        {
            var actual = await _client.InvokeAsync(x => x.AddFloat(a, b));
            var expected = new TestService().AddFloat(a, b);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public async Task ComplexType(Complex a, Complex b)
        {
            var actual = await _client.InvokeAsync(x => x.AddComplex(a, b));
            var expected = new TestService().AddComplex(a, b);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public async Task ComplexTypeArray(IEnumerable<Complex> array)
        {
            var actual = await _client.InvokeAsync(x => x.SumComplexArray(array));
            var expected = new TestService().SumComplexArray(array);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(" 2008-06-11T16:11:20.0904778Z", DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces)]
        public async Task EnumParameter(string value, DateTimeStyles styles)
        {
            var actual = await _client.InvokeAsync(x => x.ParseDate(value, styles));
            var expected = new TestService().ParseDate(value, styles);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public async Task ByteArray(byte[] input)
        {
            var actual = await _client.InvokeAsync(x => x.ReverseBytes(input));
            var expected = new TestService().ReverseBytes(input);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task AsyncOperation()
        {
            var actual = await _client.InvokeAsync(x => x.WaitAsync(500));
            Assert.True(actual >= 450);
        }

        [Fact]
        public async Task GenericParameter()
        {
            var actual = await _client.InvokeAsync(x => x.GetDefaultValue<decimal>());
            var expected = new TestService().GetDefaultValue<decimal>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ReturnVoid()
        {
            await _client.InvokeAsync(x => x.DoNothing());
        }
    }
}