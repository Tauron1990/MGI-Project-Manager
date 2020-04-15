using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public abstract class MappingEntryBase<TContext> : MappingEntry<TContext>
        where TContext : IOrginalContextProvider
    {
        private readonly Func<object, object>   _accessor;
        private readonly Action<object, object> _setter;

        protected MappingEntryBase([CanBeNull] string membername, [CanBeNull] Type targetType)
        {
            if (targetType == null || membername == null) return;

            var mem = targetType.GetMember(membername, MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .FirstOrDefault();

            if (mem == null) return;

            TargetMember = mem;

            var prop = mem as PropertyInfo;
            if (prop != null)
            {
                MemberType = prop.PropertyType;

                if (prop.CanRead) _accessor = prop.GetValue;
                if (prop.CanWrite) _setter  = prop.SetValue;
                return;
            }

            var fld = mem as FieldInfo;
            if (fld == null) return;

            MemberType = fld.FieldType;
            _accessor  = fld.GetValue;
            _setter    = fld.SetValue;
        }

        [CanBeNull]
        protected MemberInfo TargetMember { get; }

        [CanBeNull]
        protected Type MemberType { get; }

        protected void SetValue([NotNull] object target, [CanBeNull] object value) => _setter(Argument.NotNull(target, nameof(target)), value);

        [NotNull]
        protected object GetValue([NotNull] object target) => _accessor(Argument.NotNull(target, nameof(target)));

        public override Exception VerifyError()
        {
            if (_accessor == null || _setter == null)
                return new SerializerElementNullException("Member");
            return null;
        }

        public override void Progress(object target, TContext context, SerializerMode mode)
        {
            switch (mode)
            {
                case SerializerMode.Deserialize:
                    Deserialize(target, context);
                    break;
                case SerializerMode.Serialize:
                    Serialize(target, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        protected abstract void Deserialize([NotNull] object target, [NotNull] TContext context);
        protected abstract void Serialize([NotNull]   object target, [NotNull] TContext context);
    }
}