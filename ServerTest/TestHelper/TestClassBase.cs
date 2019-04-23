using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit.Abstractions;

namespace ServerTest.TestHelper
{
    public abstract class TestClassBase<TType> where TType : class
    {
        protected ITestOutputHelper TestOutputHelper { get; }

        protected TestClassBase(ITestOutputHelper testOutputHelper) 
            => TestOutputHelper = testOutputHelper;

        protected virtual TestingObject<TType> GetTestingObject() => new TestingObject<TType>();

        protected TMockType BuildMock<TMockType>(params Action<Mock<TMockType>>[] config) where TMockType : class
        {
            var mock = new Mock<TMockType>();
            foreach (var action in config)
                action(mock);
            return mock.Object;
        }

        public TContext GetDbContext<TContext>(Func<DbContextOptions, TContext> factory)
            where TContext : DbContext
        {
            var ops = new DbContextOptionsBuilder<TContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            return factory(ops);
        }
    }
}