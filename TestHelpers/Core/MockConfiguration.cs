﻿using System;
using System.Collections.Generic;
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

        public Mock<TInterface> Mock { get; } = new Mock<TInterface>();

        public MockConfiguration(ServicesConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MockConfiguration<TInterface> For(Action<Mock<TInterface>> action)
        {
            action(Mock);
            return this;
        }

        public MockConfiguration<TInterface> WithAssert(Action<Mock<TInterface>> assert)
        {
            _assert = assert;
            return this;
        }

        public ServicesConfiguration BuildService()
        {
            _configuration.ServiceEntries.Add(new MockGenericServiceEntry<TInterface>(Mock) { Asseration = _assert});
            return _configuration;
        }
    }
}