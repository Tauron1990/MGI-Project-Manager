using Xunit.Abstractions;

namespace ServerTest.TestHelper
{
    public abstract class TestBaseClass<TType> where TType : class
    {
        protected ITestOutputHelper TestOutputHelper { get; }

        protected TestBaseClass(ITestOutputHelper testOutputHelper) 
            => TestOutputHelper = testOutputHelper;

        protected virtual TestingObject<TType> GetTestingObject() => null;
    }
}