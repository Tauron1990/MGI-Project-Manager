using System;
using System.Collections.Generic;

namespace Tauron.Application.MgiProjectManager.UI
{
    public sealed class RuntimeCalculatorResult
    {
        public RuntimeCalculatorResult()
        {
            Iterations = new List<RunTimeCalculatorItem>();
        }

        public List<RunTimeCalculatorItem> Iterations { get; }

        public RunTimeCalculatorItem Setup { get; set; }

        public TimeSpan? Runtime { get; set; }
    }
}