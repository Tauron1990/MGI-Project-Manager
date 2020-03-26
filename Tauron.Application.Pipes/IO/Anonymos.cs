using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Pipes.IO
{
    public static class Anonymos
    {
        [PublicAPI]
        public static IPipe Create(PipeDirection pipeDirection, out string name)
        {
            var server = new AnonymousPipeServerStream(pipeDirection, HandleInheritability.Inheritable);
            name = server.GetClientHandleAsString();

            return new AStream(server);
        }

        [PublicAPI]
        public static IPipe Create(PipeDirection pipeDirection, string name) => new AStream(new AnonymousPipeClientStream(pipeDirection, name));

        private sealed class AStream : PipeBase
        {
            public AStream(PipeStream pipeStream) : base(pipeStream)
            {
            }

            protected override Task Connect(PipeStream stream) => Task.CompletedTask;
        }
    }
}