using System;
using System.IO;
using Tauron.Application.Files.VirtualFiles.InMemory.Data;

namespace Tauron.Application.Files.VirtualFiles.InMemory
{
    public sealed class InMemoryStream : MemoryStream
    {
        private readonly DataFile _data;

        public InMemoryStream(DataFile data)
            : base(data.Data ?? Array.Empty<byte>()) =>
            _data = data;

        protected override void Dispose(bool disposing)
        {
            _data.Data = ToArray();

            base.Dispose(disposing);
        }
    }
}