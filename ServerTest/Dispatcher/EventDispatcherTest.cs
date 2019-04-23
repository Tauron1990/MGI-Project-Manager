using System;
using System.Collections.Generic;
using System.Text;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Model;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Dispatcher
{
    public class EventDispatcherTest : TestClassBase<EventDispatcher>
    {
        private class EventHelpTest : IEventHelper
        {
            private class TestEventToken : TypedEventToken<string>
            {
                public override string EventElement => "Ok";
            }

            public EventToken GetEventToken() => new TestEventToken();
        }
        private class EventHelpTest2 : IEventHelper
        {
            private class TestEventToken : EventToken
            {
                public override Type EventType => typeof(int);
            }

            public EventToken GetEventToken() => new TestEventToken();
        }

        public EventDispatcherTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override TestingObject<EventDispatcher> GetTestingObject() 
            => base.GetTestingObject().AddDependency<IEnumerable<IEventHelper>>(new IEventHelper[]{ new EventHelpTest(), new EventHelpTest2() });

        [Fact]
        public void GetToken_RightWay_Test()
        {
            var dispatcher = GetTestingObject().GetResolvedTestingObject();

            var token = dispatcher.GetToken<string>();

            Assert.NotNull(token);
            Assert.Equal("Ok", token.EventElement);
        }

        [Fact]
        public void GetToken_WrongWay_Test()
        {
            var dispatcher = GetTestingObject().GetResolvedTestingObject();

            var token = dispatcher.GetToken<int>();

            Assert.Null(token);
        }

        [Fact]
        public void GetToken_Exception_Test()
        {
            var dispatcher = GetTestingObject().GetResolvedTestingObject();

            Assert.Throws<KeyNotFoundException>(() => dispatcher.GetToken<StringBuilder>());
        }
    }
}