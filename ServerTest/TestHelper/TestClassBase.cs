using Xunit.Abstractions;

namespace ServerTest.TestHelper
{
    public abstract class TestClassBase<TType> where TType : class
    {
        protected ITestOutputHelper TestOutputHelper { get; }

        protected TestClassBase(ITestOutputHelper testOutputHelper) 
            => TestOutputHelper = testOutputHelper;

        protected virtual TestingObject<TType> GetTestingObject() => new TestingObject<TType>();
    }
}