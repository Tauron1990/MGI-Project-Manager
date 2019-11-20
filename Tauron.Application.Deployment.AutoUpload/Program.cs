using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Functional;
using Tauron.Application.Deployment.AutoUpload.Build;

namespace Tauron.Application.Deployment.AutoUpload
{
    class Program
    {
        private static ConsoleUi _ui = new ConsoleUi();

        static async Task Main(string[] args)
        {
            ConsoleUi.Title = "Auto Versionen und Uploads";

            var context = DotNetContext.Create();
            var run = context.HasValue();

            while (run)
            {
                var operation = Option
                    .Some(Console.ReadLine())
                    .Select(s =>
                    {
                        switch (s)
                        {
                            default:
                                return new Func<DotNetContext, Task<Result<bool, Exception>>>(context => Task.FromResult(Result.Success<bool, Exception>(true)));
                        }
                    });

                await operation.DoAsync(async func 
                    => await context.DoAsync(async netContext 
                        => (await func(netContext)).Apply(b => run = b, _ui.WriteError)));
            }
        }
    }
}
