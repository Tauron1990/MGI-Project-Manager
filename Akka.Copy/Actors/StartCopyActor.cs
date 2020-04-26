using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Copy.Messages;

namespace Akka.Copy.Actors
{
    public sealed class StartCopyActor : ReceiveActor
    {
        public static Props CreateProps(ActorSystem actorSystem)
            => Props.Create<StartCopyActor>();

        private readonly IActorRef _copyPrepare;

        public StartCopyActor()
        {
            _copyPrepare = Context.ActorOf(CopyPrepareActor.CreateProps(Context.System), "Copy-Runner");
            CopyStoppedBehavior();
        }


        private void CopyStoppedBehavior() 
            => Receive<StartCopyMessage>(StartCopy);

        private void StartCopy(StartCopyMessage msg)
        {
            var error = Validate(msg).FirstOrDefault();
            if (!string.IsNullOrEmpty(error))
            {
                Context.Sender.Tell(new GenericErrorMessage(error));
                return;
            }

            _copyPrepare.Forward(msg);
        }

        private static IEnumerable<string> Validate(StartCopyMessage msg)
        {
            if (!Directory.Exists(msg.BaseDic))
                yield return "Quelle Exitiert Nicht";
            if (!Directory.Exists(msg.TargetDic)) 
                Directory.CreateDirectory(msg.TargetDic);

            foreach (var entry in msg.Entrys.ToArray())
            {
                if (Directory.Exists(Path.Combine(msg.BaseDic, entry)) || File.Exists(Path.Combine(msg.BaseDic, entry))) continue;

                yield return $"\"{entry}\" Existiert nicht";
            }
        }

        protected override SupervisorStrategy SupervisorStrategy() 
            => new OneForOneStrategy(e => Directive.Restart);

        protected override void PreRestart(Exception reason, object message) 
            => Context.Parent.Tell(new GenericErrorMessage(reason.Message));
    }
}