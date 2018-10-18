using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services.Core;
using Tauron.MgiManager.User.Service.Core;
using Tauron.MgiManager.User.Service.UserManager;
using Tauron.MgiManager.User.Shared;
using Tauron.MgiManager.User.Shared.Command;
using Tauron.MgiManager.User.Shared.Events;
using Tauron.TestHelper;
using Tauron.TestHelper.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.MgiManager.User.Service.Tests
{
    public sealed class CreateUserCommandHandlerTests
    {
        private readonly ILogger<CreateUserCommandHandler> _mockLogger;

        public CreateUserCommandHandlerTests(ITestOutputHelper testOutputHelper) 
            => _mockLogger = new MockLogger<CreateUserCommandHandler>(testOutputHelper);


        [Theory]
        [InlineData("TestUser", "TestPassword")]
        [InlineData("Short", "1")]
        [InlineData("S", "Short")]
        [InlineData("lfasjkhklafaklhfaöfhaslfjkklasflk", "kjhafkhjakklahjfasfklöhaösfhöklhjasf")]
        public async Task Test_CreateUserHandler_ValidUser(string name, string passwort)
        { 
            var commitCalled = false;
            UserAggregate aggregate = null;

            var mockSession = new MockSession(new Dictionary<Guid, AggregateRoot>(), add:o =>  aggregate = o as UserAggregate, commit:() => commitCalled = true);
            
            var handler = SCHTest.Create(new CreateUserCommandHandler(mockSession, _mockLogger));
            var command = new CreateUserCommand(name, passwort);


            await handler.Handle(command);


            Assert.True(commitCalled);
            Assert.NotNull(aggregate);
            Assert.Equal(IdGenerator.Generator.NewGuid(UserNamespace.UserNameSpace, command.Name), aggregate.Id);
            Assert.Equal(command.Name, aggregate.Name);
            Assert.NotNull(aggregate.Hash);
            Assert.NotNull(aggregate.Salt);

            var es = aggregate.FlushUncommittedChanges();
            Assert.Single(es);

            var re = es[0] as UserCreatedEvent;
            Assert.NotNull(re);
            Assert.False(re.Error); 
            Assert.Null(re.ErrorMessage);
        }

        [Theory]
        [InlineData("TestUser", "")]
        [InlineData("", "TestPassword")]
        public async Task Test_CreateUserHandler_InvalidUser(string name, string passwort)
        {
            var commitCalled = false;
            UserAggregate aggregate = null;
            UserCreatedEvent userCreatedEvent = null;

            var mockSession = new MockSession(new Dictionary<Guid, AggregateRoot>(), 
                add: o => aggregate = o as UserAggregate, 
                commit: () => commitCalled = true,
                eventPublisher:new MockEventPublisher(o => userCreatedEvent = o as UserCreatedEvent));

            var handler = SCHTest.Create(new CreateUserCommandHandler(mockSession, _mockLogger));
            var command = new CreateUserCommand(name, passwort);
            

            await handler.Handle(command);


            Assert.False(commitCalled);
            Assert.Null(aggregate);
            Assert.NotNull(userCreatedEvent);
            Assert.Null(userCreatedEvent.Salt);
            Assert.Null(userCreatedEvent.Hash);
            Assert.NotNull(userCreatedEvent.Name);
            Assert.NotNull(userCreatedEvent.ErrorMessage);
        }

        [Fact]
        public async Task Test_CreateUserHandler_DublicateUser()
        {
            const string name = "TestUser";
            const string passwort = "TestPassword";

            var id = IdGenerator.Generator.NewGuid(UserNamespace.UserNameSpace, name);
            var duplicate = new UserAggregate(id);
            var (hash, salt) = PasswordGenerator.Hash(passwort);
            duplicate.CreateUser(name, hash, salt);
            duplicate.FlushUncommittedChanges();

            var dataBase = new Dictionary<Guid, AggregateRoot>
            {
                {id,  duplicate}
            };

            var commitCalled = false;
            UserAggregate aggregate = null;
            UserCreatedEvent userCreatedEvent = null;

            var mockSession = new MockSession(dataBase,
                add: o => aggregate = o as UserAggregate,
                commit: () => commitCalled = true,
                eventPublisher: new MockEventPublisher(o => userCreatedEvent = o as UserCreatedEvent));

            var handler = SCHTest.Create(new CreateUserCommandHandler(mockSession, _mockLogger));
            var command = new CreateUserCommand(name, passwort);


            await handler.Handle(command);


            Assert.False(commitCalled);
            Assert.Null(aggregate);
            Assert.NotNull(userCreatedEvent);
            Assert.Null(userCreatedEvent.Salt);
            Assert.Null(userCreatedEvent.Hash);
            Assert.NotNull(userCreatedEvent.Name);
            Assert.NotNull(userCreatedEvent.ErrorMessage);
        }
    }
}