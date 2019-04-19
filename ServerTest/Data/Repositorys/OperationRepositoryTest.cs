using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Repositorys;
using Xunit.Abstractions;

namespace ServerTest.Data.Repositorys
{
    public class OperationRepositoryTest : TestClassBase<OperationRepository>
    {
        public OperationRepositoryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        { }

        protected override TestingObject<OperationRepository> GetTestingObject() 
            => base.GetTestingObject().AddContextDependecy(op => new ApplicationDbContext(op));


    }
}