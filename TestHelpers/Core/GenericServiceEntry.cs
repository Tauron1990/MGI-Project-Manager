using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestHelpers.Core
{
    public sealed class GenericServiceEntry<TInterface, TType> : ServiceEntry
        where TType : TInterface where TInterface : class
    {
        public GenericServiceEntry(TType service)
            => Service = service;

        public TType Service { get; }

        public Action<TType>? Asseration { get; set; }

        public override void Register(IServiceCollection collection)
            => collection.AddSingleton<TInterface>(Service);

        public override void Assert()
            => Asseration?.Invoke(Service);
    }

    public sealed class MockGenericServiceEntry<TInterface> : ServiceEntry
        where TInterface : class
    {
        private readonly Mock<TInterface> _mockService;

        public MockGenericServiceEntry(Mock<TInterface> mockService) => _mockService = mockService;

        public Action<Mock<TInterface>>? Asseration { get; set; }

        public override void Register(IServiceCollection collection)
            => collection.AddSingleton(s => _mockService.Object);

        public override void Assert()
            => Asseration?.Invoke(_mockService);
    }
}