using JetBrains.Annotations;

namespace JKang.IpcServiceFramework
{
    [PublicAPI]
    public static class TcpIpcServiceCollectionExtensions
    {
        public static IIpcServiceBuilder AddTcp(this IIpcServiceBuilder builder) => builder;
    }
}