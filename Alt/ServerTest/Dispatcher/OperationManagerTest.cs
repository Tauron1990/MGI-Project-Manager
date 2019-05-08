using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

            public bool ThrowException { private get; set; }

            public bool ExecuteCalled { get; private set; }

            public bool PostExecuteCalled { get; private set; }

            public bool RemoveCalled { get; private set; }

            public bool ErrorCalled { get; private set; }

            public Task<OperationSetup[]> Execute(Operation op)
            {
                if(PostExecuteCalled) throw new InvalidOperationException("Post Called Before Execute");
                if (ExecuteCalled) throw new InvalidOperationException("Execute Called Twice");

                ExecuteCalled = true;
                if(ThrowException)
                    throw new InvalidOperationException("Expectet Exception");

                return Task.FromResult(new []{new OperationSetup("Test", "Test", ImmutableDictionary<string, string>.Empty, DateTime.Now) });
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

        private bool _removeCalled;

        private bool _createOp;

        private TestAction _testAction;
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
                                                               .Callback<string, Action<OperationEntity>>(((s, action) => action(_createdOp))),
                                                         m => m.Setup(or => or.Remove(It.Is<string>(e => e == _createdOp.OperationId)))
                                                               .Returns(Task.CompletedTask).Callback(() => _removeCalled = true));

            _testAction = new TestAction();

            var unitMock = BuildMock<IUnitOfWork>(m => m.Setup(u => u.SaveChanges()).Returns(Task.FromResult(0)),
                                                  m => m.Verify(u => u.SaveChanges(), Times.Never),
                                                  m => m.SetupGet(u => u.OperationRepository).Returns(opRepo));
            
            return base.GetTestingObject()
                       .AddDependency<IEnumerable<IOperationAction>>(new[] { _testAction })
                       .AddLogger(TestOutputHelper)
                       .AddDependency(unitMock)
                       .AddDependencyWithProvider<IBackgroundTaskDispatcher>(p => new SyncTaskSheduler(p));
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
            _createOp = true;

            var testingObject = GetTestingObject();
            var opMan         = testingObject.GetResolvedTestingObject();

            await opMan.ExecuteNext(_createdOp.OperationId);

            Assert.True(_testAction.ExecuteCalled);
            Assert.False(_testAction.ErrorCalled);
            Assert.False(_testAction.RemoveCalled);
            Assert.True(_testAction.PostExecuteCalled);
        }

        [Fact]
        public async Task ExecuteNext_Test_Exceoption()
        {
            _createOp = true;
            _testAction.ThrowException = true;

            var testingObject = GetTestingObject();
            var opMan         = testingObject.GetResolvedTestingObject();

            await opMan.ExecuteNext(_createdOp.OperationId);

            Assert.True(_testAction.ExecuteCalled);
            Assert.True(_testAction.ErrorCalled);
            Assert.False(_testAction.RemoveCalled);
            Assert.False(_testAction.PostExecuteCalled);
        }

        [Fact]
        public async Task RemoveAction_Test()
        {
            _createOp                  = true;

            var testingObject = GetTestingObject();
            var opMan         = testingObject.GetResolvedTestingObject();

            await opMan.RemoveAction(_createdOp.OperationId);

            Assert.True(_removeCalled);
            Assert.True(_testAction.RemoveCalled);
        }

        [Fact]
        public async Task CleanUpExpiryOperation_Test()
        {
            _createOp = true;

            var testingObject = GetTestingObject();
            var opMan         = testingObject.GetResolvedTestingObject();

            await opMan.CleanUpExpiryOperation();

            Assert.True(_removeCalled);
            Assert.True(_testAction.RemoveCalled);
        }

        [Fact]
        public async Task GetOperations_Test()
        {
            _createOp = true;

            var testingObject = GetTestingObject();
            var opMan         = testingObject.GetResolvedTestingObject();
            
            var ops = await opMan.GetOperations(of =>
                                      {
                                          Assert.Equal(_createdOp.OperationId, of.OperationId);
                                          Assert.Equal(of.CurrentOperation, _createdOp.CurrentOperation);
                                          Assert.Equal(of.OperationType, _createdOp.OperationType);
                                          Assert.Equal(of.Context.Select(p => p.Key), _createdOp.Context.Select(oce => oce.Name));
                                          return true;
                                      });

            Assert.Single(ops);
            Assert.Equal(ops[0], _createdOp.OperationId);
        }

        [Fact]
        public async Task GetContext_Test()
        {
            _createOp = true;

            var testingObject = GetTestingObject();
            var opMan         = testingObject.GetResolvedTestingObject();

            var context = await opMan.GetContext(_createdOp.OperationId);

            Assert.Equal(context.Select(p => p.Key), _createdOp.Context.Select(oce => oce.Name));
        }
    }
}