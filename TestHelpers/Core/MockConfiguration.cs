using System;
using JetBrains.Annotations;
using Moq;

namespace TestHelpers.Core
{
    [PublicAPI]
    public sealed class MockConfiguration<TInterface>
        where TInterface : class
    {
        private readonly ServicesConfiguration _configuration;

        private Action<Mock<TInterface>>? _assert;

        public MockConfiguration(ServicesConfiguration configuration) => _configuration = configuration;

        private Mock<TInterface> Mock { get; } = new Mock<TInterface>();

        public MockConfiguration<TInterface> With(Action<Mock<TInterface>> action)
        {
            action(Mock);
            return this;
        }

        public MockConfiguration<TInterface> Assert(Action<Mock<TInterface>> assert)
        {
            _assert = assert;
            return this;
        }

        public ServicesConfiguration RegisterMock()
        {
            _configuration.ServiceEntries.Add(new MockGenericServiceEntry<TInterface>(Mock) {Asseration = _assert});
            return _configuration;
        }
    }
}