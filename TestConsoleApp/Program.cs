using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Tauron.Application.Pipes;
using Tauron.Application.Pipes.IO;

namespace TestConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var readServer = new PipeServer<string>(Anonymos.Create(PipeDirection.In, out var name));
            readServer.MessageRecivedEvent += ea =>
            {
                Console.Write("Recived: ");
                Console.WriteLine(ea.Message);
                return Task.CompletedTask;
            };

            await readServer.Connect();

#pragma warning disable 4014
            Task.Run(() => ReadTest(name));
#pragma warning restore 4014

            Console.ReadKey();
        }

        private static async void ReadTest(string name)
        {
            using var writeServer = new PipeServer<string>(Anonymos.Create(PipeDirection.Out, name));
            await writeServer.Connect();

            await writeServer.SendMessage($"1: {Console.ReadLine()}");
            await writeServer.SendMessage($"2: {Console.ReadLine()}");
            await writeServer.SendMessage($"3: {Console.ReadLine()}");
        }
    }
}
