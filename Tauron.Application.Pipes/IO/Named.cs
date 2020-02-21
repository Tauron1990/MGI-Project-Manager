using System.IO.Pipes;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Pipes.IO
{
    public static class Named
    {
        [PublicAPI]
        public static IPipe Client(string name)
        {
            return new ClientImpl(name);
        }

        [PublicAPI]
        public static IPipe Server(string name)
        {
            return new ServerImpl(name);
        }

        private class ClientImpl : PipeBase
        {
            public ClientImpl(string name)
                : base(new NamedPipeClientStream(name))
            {
            }

            protected override async Task Connect(PipeStream stream)
            {
                await ((NamedPipeClientStream) stream).ConnectAsync();
            }
        }

        private class ServerImpl : PipeBase
        {
            public ServerImpl(string name)
                : base(new NamedPipeServerStream(name))
            {
            }

            protected override async Task Connect(PipeStream stream)
            {
                await ((NamedPipeServerStream) stream).WaitForConnectionAsync();
            }
        }
    }
}