using System.IO;
using System.Threading;

namespace Akka.Copy.Messages
{
    public sealed class CopyFileMessage
    {
        public string TargetDic { get; }

        public string BaseDic { get; }

        public FileInfo Source { get; }

        public CancellationToken Token { get; }

        public CopyFileMessage(string targetDic, FileInfo source, CancellationToken token, string baseDic)
        {
            TargetDic = targetDic;
            Source = source;
            Token = token;
            BaseDic = baseDic;
        }
    }
}