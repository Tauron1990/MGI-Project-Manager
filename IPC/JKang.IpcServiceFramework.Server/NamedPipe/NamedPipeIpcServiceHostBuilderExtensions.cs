using JetBrains.Annotations;
using JKang.IpcServiceFramework.NamedPipe;

namespace JKang.IpcServiceFramework
{
    [PublicAPI]
    public static class NamedPipeIpcServiceHostBuilderExtensions
    {
        public static IpcServiceHostBuilder AddNamedPipeEndpoint<TContract>(this IpcServiceHostBuilder builder,
            string name, string pipeName)
            where TContract : class =>
            builder.AddEndpoint(new NamedPipeIpcServiceEndpoint<TContract>(name, builder.ServiceProvider, pipeName));
    }
}