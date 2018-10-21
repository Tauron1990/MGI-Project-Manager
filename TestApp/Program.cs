using System;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //var testObject = new UserCreatedEvent("test", "test", "Test", Guid.NewGuid());

            //var test2 = testObject.Clear();

            var test = typeof(Program);
            Console.WriteLine(test.AssemblyQualifiedName);
            Console.WriteLine($"{test.FullName}, {test.Assembly.FullName}");

            Console.ReadKey();
        }

    }
}
