using System;
using System.Threading.Tasks;
using Tauron.ServiceBootstrapper;

namespace CalculatorService
{
    class Program
    {
        static async Task Main(string[] args) 
            => await BootStrapper.Run<Empty>(args);
    }
}
