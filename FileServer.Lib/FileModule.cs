using FileServer.Lib.Impl;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Shared;

namespace FileServer.Lib
{
    public sealed class FileModule : DIModule
    {
        public override void Load()
        {
            this.AddScoped<IFileManager, FileManager>();
        }
    }
}