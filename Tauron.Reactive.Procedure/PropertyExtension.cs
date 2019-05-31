using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace Tauron.Reactive.Procedure
{
    [PublicAPI]
    public static class PropertyExtension
    {
        public static PropertySubject<TType> CreatePropertySubject<TType>(this SubjectBase<TType> subject) => new PropertySubject<TType>(subject);
    }
}