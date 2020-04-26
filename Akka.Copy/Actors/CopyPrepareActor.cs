using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Copy.Core;
using Akka.Copy.Messages;

namespace Akka.Copy.Actors
{
    public sealed class CopyPrepareActor : ReceiveActor
    {
        public static Props CreateProps(ActorSystem actorSystem)
            => Props.Create<CopyPrepareActor>();

        private readonly IActorRef _runCopy;

        public CopyPrepareActor()
        {
            Receive<StartCopyMessage>(PrepareCopy);
            _runCopy = Context.ActorOf(RunCopyActor.CreateProps(Context.System), "Run-Copy");
        }

        private void PrepareCopy(StartCopyMessage msg)
        {
            var context = Context;
            var files = new ConcurrentBag<FileInfo>();

            var locations = msg.Entrys;
            var baseDic = msg.BaseDic;
            var token = msg.CancellationToken;
            long maxSize = 0;

            void ErrorHandler(Exception e) => context.Sender.Tell(new RecuverErrorMessage(e));

            var result = Parallel.ForEach(locations.Where(s => !string.IsNullOrWhiteSpace(s))
                   .Select(location => Path.Combine(baseDic, location))
                   .Where(s =>
                          {
                              try
                              {
                                  return Directory.Exists(s) || File.Exists(s);
                              }
                              catch (Exception e)
                              {
                                  ErrorHandler(e);
                                  return false;
                              }
                          })
                   .SelectMany(full =>
                               {
                                   try
                                   {
                                       return Path.HasExtension(full) ? new[] {Path.Combine(baseDic, full)} : full.TraverseTree(ErrorHandler);
                                   }
                                   catch (Exception e) when (e is UnauthorizedAccessException)
                                   {
                                       ErrorHandler(e);
                                       return Enumerable.Empty<string>();
                                   }
                               }).ErrorProof(ErrorHandler),
                (file, state) =>
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            state.Stop();
                            return;
                        }

                        var info = new FileInfo(file);

                        if (!info.Exists) return;

                        files.Add(info);

                        var cursize = Interlocked.Add(ref maxSize, info.Length);
                        var label = $"{files.Count} - {cursize.Gigabytes()}";

                        context.Sender.Tell(new UpdateMessage(label, -1));
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        ErrorHandler(e);
                    }
                });

            if(result.IsCompleted) 
                _runCopy.Forward(new ReadyCopyMessage(files.ToArray(), msg.TargetDic, token, maxSize, msg.BaseDic));
            else
            {
                Context.Sender.Tell(new GenericErrorMessage("Kopieren Abgebrochen"));
                context.Sender.Tell(new CopyCompledMessage());
            }
        }

        protected override SupervisorStrategy SupervisorStrategy() => new OneForOneStrategy(e => e is OperationCanceledException ? Directive.Stop : Directive.Escalate);
    }
}