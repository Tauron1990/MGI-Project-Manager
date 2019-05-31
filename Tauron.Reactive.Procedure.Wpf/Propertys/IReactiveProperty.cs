using System;

namespace Tauron.Reactive.Procedure.Wpf.Propertys
{
    public interface IReactiveProperty : IDisposable
    {
        object Value { get; }
    }
}