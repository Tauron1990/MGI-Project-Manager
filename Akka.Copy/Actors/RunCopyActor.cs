using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Copy.Core;
using Akka.Copy.Messages;

namespace Akka.Copy.Actors
{
    public class RunCopyActor : ReceiveActor
    {
        public static Props CreateProps(ActorSystem actorSystem)
            => Props.Create<RunCopyActor>();

        private readonly IActorRef _copyThread;
        private int _currentCount;
        private int _max;

        private long _maxSize;
        private long _currentSize;
        private int _skiped;

        private IActorRef _sender;

        public RunCopyActor()
        {
            Receive<ReadyCopyMessage>(RunCopy);
            Receive<CopyFileCompledMessage>(FileCopyCompled);
            Receive<SingleCopyUpdate>(CreateUpdate);

            Receive<UpdateMessage>(m => _sender?.Tell(m));
            Receive<NewFileCopyStartMessage>(m => _sender?.Tell(m));
            Receive<RecuverErrorMessage>(m => _sender?.Tell(m));

            _copyThread = Context.ActorOf(CopyActor.CreateProps(Context.System), "Copy-Thread");
        }

        private void CreateUpdate(SingleCopyUpdate msg)
        {
            if (msg.Skiped)
                Interlocked.Increment(ref _skiped);
            var currsize = Interlocked.Add(ref _currentSize, msg.Size);
            var count = _max - _currentCount;
            var percent = 100d / _max * count;

            var label = $"Daten: {_max - count} ---- {percent:F3}/100% -- {currsize.Gigabytes()}/{_maxSize.Gigabytes()} -- {_skiped} Übersprungen";

            _sender?.Tell(new UpdateMessage(label, (int)percent));
        }

        private void FileCopyCompled(CopyFileCompledMessage obj)
        {
            Interlocked.Decrement(ref _currentCount);

            if (_currentCount != 0) return;

            _sender?.Tell(new UpdateMessage("Fertig", 100));
            _sender?.Tell(new CopyCompledMessage());
        }

        private void RunCopy(ReadyCopyMessage obj)
        {
            _sender = Context.Sender;

            _max = obj.Files.Length;
            _maxSize = obj.MaxSize;
            _skiped = 0;
            _currentCount = 0;
            _currentSize = 0;

            var files = obj.Files;
            var token = obj.Token;

            foreach (var s in files)
            {
                try
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!s.Exists) return;

                    Interlocked.Increment(ref _currentCount);
                    _copyThread.Tell(new CopyFileMessage(obj.TargetDic, s, token, obj.BaseDic));
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    Context.Sender.Tell(new RecuverErrorMessage(e));
                }
            }
        }



        protected override SupervisorStrategy SupervisorStrategy() => new OneForOneStrategy(e => Directive.Escalate);
    }
}