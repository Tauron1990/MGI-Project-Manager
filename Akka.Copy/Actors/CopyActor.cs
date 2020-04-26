using System;
using System.IO;
using Akka.Actor;
using Akka.Copy.Messages;
using Akka.Routing;

namespace Akka.Copy.Actors
{
    public sealed class CopyActor : ReceiveActor
    {
        public static Props CreateProps(ActorSystem actorSystem)
            => Props.Create<CopyActor>().WithRouter(new SmallestMailboxPool(4));

        private readonly string _id = Guid.NewGuid().ToString();
        private readonly byte[] _buffer = new byte[short.MaxValue * 2];

        public CopyActor() 
            => Receive<CopyFileMessage>(CopyFile);

        private void CopyFile(CopyFileMessage msg)
        {
            var size = msg.Source.Length;
            var token = msg.Token;
            var baseDic = msg.BaseDic;
            var destDic = msg.TargetDic;
            
            try
            {
                if (token.IsCancellationRequested) return;

                var target = msg.Source.FullName.Replace(baseDic, destDic);
                var targetInfo = new FileInfo(target);
                if (targetInfo.Exists)
                {
                    //IncrementUpdate(size, true);
                    //continue;
                    if (msg.Source.Length == targetInfo.Length)
                    {
                        Context.Sender.Tell(new SingleCopyUpdate(true, size));
                        return;
                    }
                }

                var targetDic = targetInfo.Directory;
                if (targetDic == null)
                {
                    Context.Sender.Tell(new SingleCopyUpdate(true, size));
                    return;
                }

                if (!targetDic.Exists)
                    targetDic.Create();
                using (var stream = msg.Source.Open(FileMode.Open))
                {
                    using (var destStream = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        Context.Sender.Tell(new NewFileCopyStartMessage(_id, msg.Source.Name));

                        int readed;

                        do
                        {
                            if (token.IsCancellationRequested)
                                break;

                            readed = stream.Read(_buffer, 0, _buffer.Length);
                            if(readed == 0) continue;
                            destStream.Write(_buffer, 0, readed);

                            Context.Sender.Tell(new SingleCopyUpdate(false, readed));
                        } while (readed != 0);
                    }
                }
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Context.Sender.Tell(new RecuverErrorMessage(e));
            }
            finally
            {
                Context.Sender.Tell(new CopyFileCompledMessage());
            }
        }
    }
}