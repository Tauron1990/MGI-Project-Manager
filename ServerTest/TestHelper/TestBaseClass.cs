using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace ServerTest.TestHelper
{
    public abstract class TestBaseClass
    {
        protected ITestOutputHelper TestOutputHelper { get; }

        protected TestBaseClass(ITestOutputHelper testOutputHelper) 
            => TestOutputHelper = testOutputHelper;

        protected void AddLogger<TType>(TestingObject<TType> testingObject) where TType : class 
            => testingObject.AddDependency<ILogger<TType>>(TestOutputHelper.BuildLoggerFor<TType>());

        protected IEnumerable<Action<Mock<TType>>> CreateMockBuilder<TType>(params Action<Mock<TType>>[] config) where TType : class 
            => config;

        protected Mock<TType> BuildMock<TType>(IEnumerable<Action<Mock<TType>>> config) where TType : class
        {
            var mock = new Mock<TType>();

            foreach (var action in config)
                action(mock);
            return mock;
        }
    }

}