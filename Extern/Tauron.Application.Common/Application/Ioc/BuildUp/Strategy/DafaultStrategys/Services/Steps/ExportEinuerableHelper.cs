using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ExportEnumeratorHelper
    {
        private readonly ReflectionContext _context;
        private readonly IEnumerator<ExportMetadata> _metaEnumerator;
        private bool _ok;

        public ExportEnumeratorHelper([NotNull] IEnumerator<ExportMetadata> metaEnumerator, [NotNull] ReflectionContext context)
        {
            _metaEnumerator = metaEnumerator;
            _context = context;
        }

        public StepId NextId => _ok ? StepId.None : StepId.LoopEnd;

        public bool MoveNext()
        {
            _ok = _metaEnumerator.MoveNext();
            _context.ExportMetadataOverride = _ok ? _metaEnumerator.Current : null;

            return _ok;
        }
    }
}