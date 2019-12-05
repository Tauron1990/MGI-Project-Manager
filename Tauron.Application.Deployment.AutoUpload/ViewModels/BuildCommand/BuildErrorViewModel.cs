using System;
using System.Threading.Tasks;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildErrorViewModel))]
    public sealed class BuildErrorViewModel : OperationViewModel<BuildOperationContext>
    {
        public int ErrorCount { get; set; }

        public int ResultCode { get; set; }

        public string Console { get; set; } = string.Empty;

        protected override async Task InitializeAsync()
        {
            var failed = Context.Failed;

            if (failed == null)
            {
                await OnReturn();
                return;
            }

            ErrorCount = failed.ErrorCount;
            ResultCode = failed.Result;
            Console = failed.Console;

            await base.InitializeAsync();
        }

        [CommandTarget]
        public async Task OnNext() 
            => await OnReturn();
    }
}