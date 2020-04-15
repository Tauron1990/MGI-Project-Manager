using System;
using Microsoft.Extensions.DependencyInjection;

namespace JKang.IpcServiceFramework
{
    public static class NamedPipeIpcServiceCollectionExtensions
    {
        public static IIpcServiceBuilder AddNamedPipe(this IIpcServiceBuilder builder) => builder.AddNamedPipe(null);

        public static IIpcServiceBuilder AddNamedPipe(this IIpcServiceBuilder builder, Action<NamedPipeOptions>? configure)
        {
            var options = new NamedPipeOptions();
            configure?.Invoke(options);

            builder.Services
               .AddSingleton(options)
                ;

            return builder;
        }
    }
}