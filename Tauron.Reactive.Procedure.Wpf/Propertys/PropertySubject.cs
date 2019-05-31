using System;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ReactiveUI;

namespace Tauron.Reactive.Procedure.Wpf.Propertys
{
    [PublicAPI]
    public sealed class PropertySubject<TType> : SubjectBase<TType>, IReactiveProperty
    {
        private readonly SubjectBase<TType> _subject;

        public ObservableAsPropertyHelper<TType> Property { get; }
        public TType Value => Property.Value;

        public PropertySubject(SubjectBase<TType> subject, ReactiveObject source, string name)
        {
            _subject = subject;
            Property = subject.ToProperty(source, name);
        }

        public PropertySubject(ReactiveObject source, string name)
            : this(new Subject<TType>(), source, name)
        {}

        public override void Dispose()
        {
            Property.Dispose();
            _subject.Dispose();
        }

        public override void OnCompleted() => _subject.OnCompleted();

        public override void OnError(Exception error) => _subject.OnError(error);

        public override void OnNext(TType value) => _subject.OnNext(value);

        public override IDisposable Subscribe(IObserver<TType> observer) => _subject.Subscribe(observer);

        public override bool HasObservers => _subject.HasObservers;

        public override bool IsDisposed => _subject.IsDisposed;

        object IReactiveProperty.Value => Property.Value;
    }
}