using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Tauron.Application.Deployment.Server.Services.Validatoren;

namespace Tauron.Application.Deployment.Server.Services
{
    public sealed class DownloadServiceImpl : DownloadService.DownloadServiceBase
    {
        public override async Task Download(DownloadEntry request, IServerStreamWriter<ByteMessage> responseStream, ServerCallContext context)
        {
            const int messageSize = 125000; //1 MegaByte

            await DownloadEntryValidator.ForAsync(request);

            Uri url = new Uri(request.Url);

            await using var stream = url.IsFile ? File.OpenRead(url.LocalPath) : await OpenWebStream(url);
            await using var crcStream = new CrcStream(stream);

            byte[] buffer = new byte[messageSize];
            int count;

            do
            {
                count = await crcStream.ReadAsync(buffer, 0, buffer.Length);
                if (count == 0) break;
                
                await responseStream.WriteAsync(new ByteMessage
                                                {
                                                    CRC = crcStream.ReadCrc,
                                                    Data = ByteString.CopyFrom(buffer.AsSpan().Slice(0, count))
                                                });
            } while (count == 0);
        }

        private static async Task<Stream> OpenWebStream(Uri url)
        {
            var request = WebRequest.Create(url);
            var respones = await request.GetResponseAsync();
            return respones.GetResponseStream();
        }
    }
}