using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new NamedPipeServerStream("test", PipeDirection.In);
            test.co
            Reader(test);
            Thread.Sleep(20000);
            test.Dispose();

            Console.ReadKey();
        }

        private static async void Reader(PipeStream reader)
        {
            var temp = await reader.ReadAsync(new byte[4096], 0, 4096);
            Console.WriteLine("Write Compled: " + temp);
        }
    }
}
