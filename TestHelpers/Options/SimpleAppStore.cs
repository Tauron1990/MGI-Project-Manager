using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tauron.Application.OptionsStore;

namespace TestHelpers.Options
{
    public sealed class SimpleOptionStore : IOptionsStore
    {
        private readonly Dictionary<string, IAppOptions> _appOptionses = new Dictionary<string, IAppOptions>();

        public SimpleOptionStore(params (string appName, IAppOptions appOptions)[] options)
        {
            foreach (var (appName, appOptions) in options) 
                _appOptionses[appName] = appOptions;
        }

        public IAppOptions GetAppOptions(string applicationName)
        {
            if (_appOptionses.TryGetValue(applicationName, out var opt))
                return opt;

            return _appOptionses.Count == 1 ? _appOptionses.Values.First() : new SimpleAppOptions(applicationName);
        }

        public static SimpleOptionStore CreateSimple(IOption option) 
            => new SimpleOptionStore(("", new SimpleAppOptions("", options: (option.Key, option))));

        public static SimpleOptionStore CreateSimple()
            => new SimpleOptionStore();
    }
}