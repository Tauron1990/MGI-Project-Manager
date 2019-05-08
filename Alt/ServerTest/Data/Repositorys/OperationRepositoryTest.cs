using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Data.Repositorys;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data.Repositorys
{
    public class OperationRepositoryTest : TestClassBase<OperationRepository>
    {
        private static readonly string TestId1 = Guid.NewGuid().ToString();
        private static readonly string TestId2 = Guid.NewGuid().ToString();
        private static readonly string TestId3 = Guid.NewGuid().ToString();
        private static readonly string TestId4 = Guid.NewGuid().ToString();

        public OperationRepositoryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override TestingObject<OperationRepository> GetTestingObject()
            => base.GetTestingObject().AddContextDependecy(op => new ApplicationDbContext(op));

        private OperationEntity CreateOperationEntity(string id, bool compled = false)
        {
            return new OperationEntity
                   {
                       CurrentOperation = "Test",
                       Compled          = compled,
                       Context = new List<OperationContextEntity>
                                 {
                                     new OperationContextEntity
                                     {
                                         Name  = "TestKey",
                                         Value = "Test"
                                     }
                                 },
                       ExpiryDate    = DateTime.Now + TimeSpan.FromDays(30),
                       OperationId   = id,
                       OperationType = "TestOp"
                   };
        }

        private async Task FillDataBase(TestingObject<OperationRepository> testingObject)
        {
            var context = testingObject.GetDependency<ApplicationDbContext>();

            await context.Operations.AddRangeAsync(CreateOperationEntity(TestId1), CreateOperationEntity(TestId2),
                                                   CreateOperationEntity(TestId3), CreateOperationEntity(TestId4, true));
        }

        [Fact]
        public async Task GetAllOperations_Test()
        {
            var testObject = GetTestingObject();
            var repo       = testObject.GetResolvedTestingObject();

            await FillDataBase(testObject);

            var ops = await repo.GetAllOperations();

            Assert.Equal(3, ops.Count());
        }

        [Fact]
        public async Task Find_Test()
        {
            var testObject = GetTestingObject();
            var repo       = testObject.GetResolvedTestingObject();

            await FillDataBase(testObject);

            var op = await repo.Find(TestId4);

            Assert.NotNull(op);
            Assert.True(op.Compled);
            Assert.Single(op.Context);
        }

        [Fact]
        public async Task CompledOperation_Test()
        {
            var testObject = GetTestingObject();
            var repo       = testObject.GetResolvedTestingObject();
            var context    = testObject.GetDependency<ApplicationDbContext>();

            await FillDataBase(testObject);

            var newOp = CreateOperationEntity(Guid.NewGuid().ToString());
            await repo.AddOperation(newOp);

            Assert.NotNull(context.Operations.FirstOrDefault(o => o.OperationId == newOp.OperationId));
        }

        [Fact]
        public async Task Remove_Test()
        {
            var testObject = GetTestingObject();
            var repo       = testObject.GetResolvedTestingObject();
            var context    = testObject.GetDependency<ApplicationDbContext>();

            await FillDataBase(testObject);

            await repo.Remove(TestId1);

            var removedOp = context.Operations.First(o => o.OperationId == TestId1);


            Assert.True(removedOp.Compled);
            Assert.True(removedOp.Removed);
        }

        [Fact]
        public async Task UpdateOperation_Test()
        {
            var testObject = GetTestingObject();
            var repo       = testObject.GetResolvedTestingObject();
            var context    = testObject.GetDependency<ApplicationDbContext>();

            await FillDataBase(testObject);

            await repo.UpdateOperation(TestId1, entity => entity.Context.Add(new OperationContextEntity {Name = "TestKey2", Value = "TestValue2"}));
        }
    }
}