using System.IO;
using System.Threading;

namespace Akka.Copy.Messages
{
    public sealed class ReadyCopyMessage
    {
        public FileInfo[] Files { get; }

        public string TargetDic { get; }

        public string BaseDic { get; }

        public long MaxSize { get; }

        public CancellationToken Token { get; }

        public ReadyCopyMessage(FileInfo[] files, string targetDic, CancellationToken token, long maxSize, string baseDic)
        {
            Files = files;
            TargetDic = targetDic;
            Token = token;
            MaxSize = maxSize;
            BaseDic = baseDic;
        }
    }
}