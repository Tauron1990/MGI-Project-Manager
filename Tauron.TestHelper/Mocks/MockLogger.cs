using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tauron.TestHelper.Mocks
{
    public class MockLogger<TCategory> : ILogger<TCategory>
    {
        private readonly ITestOutputHelper _outputHelper;

        private class NullDispose : IDisposable
        {
            public void Dispose()
            {
            }
        }

        public MockLogger()
        {
            
        }

        public MockLogger(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _outputHelper?.WriteLine($"{eventId}: {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NullDispose();
        }
    }
}