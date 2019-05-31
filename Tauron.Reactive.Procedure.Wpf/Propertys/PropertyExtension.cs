using System.Reactive.Subjects;
using JetBrains.Annotations;
using ReactiveUI;

namespace Tauron.Reactive.Procedure.Wpf.Propertys
{
    [PublicAPI]
    public static class PropertyExtension
    {
        public static PropertySubject<TType> CreatePropertySubject<TType>(this SubjectBase<TType> subject, ReactiveObject source, string name) 
            => new PropertySubject<TType>(subject, source, name);
    }
}