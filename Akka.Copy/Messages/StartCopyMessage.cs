using System.IO;
using System.Threading;

namespace Akka.Copy.Messages
{
    public sealed class StartCopyMessage
    {
        public string BaseDic { get; }

        public string TargetDic { get; }

        public string[] Entrys { get; }

        public CancellationToken CancellationToken { get; }

        public StartCopyMessage(string baseDic, string targetDic, string[] entrys, CancellationToken cancellationToken)
        {
            BaseDic = baseDic;
            TargetDic = targetDic;
            Entrys = entrys.Length == 0 ? Directory.GetFileSystemEntries(baseDic) : entrys;

            CancellationToken = cancellationToken;
        }

    }
}