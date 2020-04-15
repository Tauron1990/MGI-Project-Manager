using System;
using System.Reflection;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public class ObjectBuilder
    {
        public ObjectBuilder(Type? target)
        {
            if (target == null) return;

            var count = -1;
            ConstructorInfo? info = null;

            foreach (var con in target.GetConstructors())
            {
                count = con.GetParameters().Count();
                info = count switch
                {
                    0 => con,
                    1 => con,
                    _ => info
                };
            }

            if (info == null) return;

            SetConstructor(info, count);
        }

        public object? CustomObject { get; set; }

        public Func<object?, object>? BuilderFunc { get; set; }

        public void SetConstructor(ConstructorInfo? info, int parmCount)
        {
            BuilderFunc = null;

            if (info == null) return;

            var single = parmCount == 0;

            if (parmCount == -1)
                single = info.GetParameters().Length switch
                {
                    0 => true,
                    1 => false,
                    _ => single
                };

            if (parmCount > 1) return;

            if (single) BuilderFunc = o => info.Invoke(null);
            else BuilderFunc = o => info.Invoke(new[] {o});
        }

        public Exception? Verfiy()
        {
            return BuilderFunc == null ? new SerializerElementNullException("BuilderFunc") : null;
        }
    }
}