using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace Tauron.Reactive.Procedure
{
    [PublicAPI]
    public sealed class PropertySubject<TType>
    {
        private readonly SubjectBase<TType> _subject;
        private readonly Property

        public PropertySubject(SubjectBase<TType> subject)
        {
            _subject = subject;
        }

        public PropertySubject()
        {
            
        }
    }
}