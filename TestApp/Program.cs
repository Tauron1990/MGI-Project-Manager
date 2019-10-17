using System;
using System.Threading.Tasks;
using Tauron.CQRS.Services.Core;
using Tauron.MgiManager.User.Shared.Events;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var testObject = new UserCreatedEvent("test", "test", "Test", Guid.NewGuid());

            var test2 = testObject.Clear();

            Console.ReadKey();
        }
    }
}
