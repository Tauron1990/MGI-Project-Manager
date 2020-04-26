using System.IO;
using Akka.Actor;
using Akka.Configuration;

namespace Akka.Copy.App
{
    public static class CoreSystem
    {
        public static readonly ActorSystem System = ActorSystem.Create("Copy-System", ConfigurationFactory.ParseString(File.ReadAllText("akka.Copy.config.hocon")));
    }
}