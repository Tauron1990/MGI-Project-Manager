using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;


namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public class ObjectBuilder
    {
        public ObjectBuilder([CanBeNull] Type target)
        {
            if (target == null) return;

            var count = -1;
            ConstructorInfo info = null;

            foreach (var con in target.GetConstructors())
            {
                count = con.GetParameters().Count();
                switch (count)
                {
                    case 0:
                        info = con;
                        break;
                    case 1:
                        info = con;
                        break;
                }
            }

            if (info == null) return;

            SetConstructor(info, count);
        }

        [CanBeNull]
        public object CustomObject { get; set; }

        [CanBeNull]
        public Func<object, object> BuilderFunc { get; set; }

        public void SetConstructor([CanBeNull] ConstructorInfo info, int parmCount)
        {
            BuilderFunc = null;

            if (info == null) return;

            var single = parmCount == 0;

            if (parmCount == -1)
            {
                switch (info.GetParameters().Length) {
                    case 0:
                        single = true;
                        break;
                    case 1:
                        single = false;
                        break;
                }
            }

            if (parmCount > 1) return;

            if (single) BuilderFunc = o => info.Invoke(null);
            else BuilderFunc = o => info.Invoke(new[] {o});
        }

        [CanBeNull]
        public Exception Verfiy() => BuilderFunc == null ? new SerializerElementNullException("BuilderFunc") : null;
    }
}