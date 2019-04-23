using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Actions;
using Tauron.MgiProjectManager.Dispatcher.Model;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Dispatcher
{
    public class OperationManagerTest : TestClassBase<OperationManager>
    {
        private class TestAction : IOperationAction
        {
            public string Name => "Test";

            public static bool ThrowException { get; set; }

            public static bool ExecuteCalled { get; set; }

            public static bool PostExecuteCalled { get; set; }

            public static bool RemoveCalled { get; set; }

            public static bool ErrorCalled { get; set; }

            public Task<OperationSetup[]> Execute(Operation op)
            {
                if(PostExecuteCalled) throw new InvalidOperationException("Post Called Before Execute");
                if (ExecuteCalled) throw new InvalidOperationException("Execute Called Twice");

                ExecuteCalled = true;
                if(ThrowException)
                    throw new InvalidOperationException("Expectet Exception");

                return Task.FromResult(Array.Empty<OperationSetup>());
            }

            public Task PostExecute(Operation op)
            {
                if(!ExecuteCalled) throw new InvalidOperationException("Post Execute Called before Execute");
                PostExecuteCalled = true;

                return Task.CompletedTask;
            }

            public Task<bool> Remove(Operation op)
            {
                RemoveCalled = true;

                return Task.FromResult(true);
            }

            public Task Error(Operation op, Exception e)
            {
                ErrorCalled = true;

                return Task.CompletedTask;
            }
        }

        private bool _createOp = false;

        private OperationEntity _createdOp;

        public OperationManagerTest(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper) { }

        protected override TestingObject<OperationManager> GetTestingObject()
        {
            if(_createOp)
            {
                _createdOp = new OperationSetup("TestOperation", "Test", new Dictionary<string, string>
                                                                         {
                                                                             {"Test1", "Test1"},
                                                                             {"Test2", "Test2"},
                                                                             {"Test3", "Test3"},
                                                                             {"Test4", "Test4"}
                                                                         }, DateTime.Now).ToOperation().ToOperationEntity();
            }

            var opRepo = BuildMock<IOperationRepository>(m => m.Setup(or => or.AddOperation(It.IsAny<OperationEntity>()))
                                                               .Returns(Task.CompletedTask)
                                                               .Callback<OperationEntity>(op => _createdOp = op),
                                                         m => m.Setup(or => or.UpdateOperation(It.IsAny<string>(), It.IsAny<Action<OperationEntity>>()))
                                                               .Returns(Task.CompletedTask)
                                                               .Callback<string, Action<OperationEntity>>(((s, action) => action(_createdOp))));

            var unitMock = BuildMock<IUnitOfWork>(m => m.Setup(u => u.SaveChanges()).Returns(Task.FromResult(0)),
                                                  m => m.Verify(u => u.SaveChanges(), Times.Never),
                                                  m => m.SetupGet(u => u.OperationRepository).Returns(opRepo));

            return base.GetTestingObject()
                       .AddDependency<IEnumerable<IOperationAction>>(new[] {new TestAction()})
                       .AddLogger(TestOutputHelper)
                       .AddDependency(unitMock);
        }

        [Fact]
        public async Task AddOperation_Test()
        {
            var testingObject = GetTestingObject();
            var opMan = testingObject.GetResolvedTestingObject();

            const string targetOperationName = "TestOperation";

            var testSetup = new OperationSetup(targetOperationName, "Test", new Dictionary<string, string> {{"Test", "Test"}}, DateTime.Now);

            var test = await opMan.AddOperation(testSetup);

            Assert.NotNull(_createdOp);
            Assert.Equal(test, _createdOp.OperationId);
            Assert.Equal(targetOperationName, _createdOp.CurrentOperation);
            Assert.Equal("Test", _createdOp.Context.Single().Value);
        }

        [Fact]
        public async Task UpdateOperation_Test()
        {
            _createOp = true;

            var testingObject = GetTestingObject();
            var opMan         = testingObject.GetResolvedTestingObject();

            await opMan.UpdateOperation(_createdOp.OperationId, dictionary =>
                                                                {
                                                                    dictionary.Remove("Test2");
                                                                    dictionary.Add("Test5", "Test5");
                                                                });

            Assert.Null(_createdOp.Context.Find(oce => oce.Name == "Test2"));
            Assert.NotNull(_createdOp.Context.Find(oce => oce.Name == "Test5"));
        }

        [Fact]
        public async Task SearchOperation_Test()
        {
            _createOp = true;

            var testingObject = GetTestingObject();
            var opMan = testingObject.GetResolvedTestingObject();

            var result = await opMan.SearchOperation(_createdOp.OperationId);

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Equal("Test1", result["Test1"]);
        }

        [Fact]
        public async Task ExecuteNext_Test()
        {

        }
    }
}