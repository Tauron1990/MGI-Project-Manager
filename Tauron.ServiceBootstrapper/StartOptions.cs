using System;
using JetBrains.Annotations;
using Ookii.CommandLine;

namespace Tauron.ServiceBootstrapper
{
    [PublicAPI]
    public class StartOptions<TArgs>
    {
        public TArgs Args { get; }

        public CommandLineArgumentException Error { get; }

        public bool Ok { get; set; }

        public StartOptions(string[] args)
        {
            try
            {
                var parser = new CommandLineParser(typeof(TArgs));

                Args = (TArgs) parser.Parse(args);
                Ok = true;
            }
            catch (CommandLineArgumentException e)
            {
                Ok = false;
                Error = e;
            }
        }
    }
}