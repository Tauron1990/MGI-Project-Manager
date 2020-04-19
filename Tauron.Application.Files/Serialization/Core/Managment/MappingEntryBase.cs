using System;
using System.Linq;
using System.Reflection;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public abstract class MappingEntryBase<TContext> : MappingEntry<TContext>
        where TContext : IOrginalContextProvider
    {
        private readonly Func<object?, object>? _accessor;
        private readonly Action<object, object?>? _setter;

        protected MappingEntryBase(string? membername, Type? targetType)
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
                if (prop.CanWrite) _setter = prop.SetValue;
                return;
            }

            var fld = mem as FieldInfo;
            if (fld == null) return;

            MemberType = fld.FieldType;
            _accessor = fld.GetValue;
            _setter = fld.SetValue;
        }

        protected MemberInfo? TargetMember { get; }

        protected Type? MemberType { get; }

        protected void SetValue(object target, object? value) 
            => _setter?.Invoke(Argument.NotNull(target, nameof(target)), value);

        protected object? GetValue(object target) 
            => _accessor?.Invoke(Argument.NotNull(target, nameof(target)));

        public override Exception? VerifyError()
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

        protected abstract void Deserialize(object target, TContext context);
        protected abstract void Serialize(object target, TContext context);
    }
}