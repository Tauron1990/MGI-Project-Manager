using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.CQRS.Services.Core;
using Tauron.MgiManager.User.Shared.Events;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var testObject = new UserCreatedEvent("test", "test", "Test", Guid.NewGuid());

            //var test2 = testObject.Clear();

            var test = await WOW();

            

            Console.ReadKey();
        }

        private static Task<string> Test() 
            => Task.FromResult("Hallo Welt");

        private static Task WOW() 
            => Test();
    }
}
