using System;
using System.IO;

namespace FileServer.Lib.Impl.Operations
{
    public sealed class StreamFileOperation : OperationBase
    {
        private readonly Action<string> _delete;

        private bool _isCompled;

        private readonly string _path;

        public StreamFileOperation(string path, Stream stream, Action<string> delete)
        {
            _path = path;
            Stream = stream;
            _delete = delete;
        }

        public override TimeSpan Timeout => TimeSpan.FromMinutes(5);

        public Stream Stream { get; }

        protected override void DisposeImpl()
        {
            Stream.Dispose();

            if(_isCompled) return;

            _delete(_path);
        }

        protected override void FinishOperation() 
            => _isCompled = true;
    }
}