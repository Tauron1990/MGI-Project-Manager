#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>The reflection extensions.</summary>
    [PublicAPI]
    public static class ReflectionExtensions
    {
        #region Constants

        /// <summary>The default binding flags.</summary>
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        #endregion

        public static T ParseEnum<T>([NotNull] this string value, bool ignoreCase)
            where T : struct
        {
            T evalue;
            return Enum.TryParse(value, ignoreCase, out evalue) ? evalue : default(T);
        }

        #region Public Methods and Operators

        /// <summary>
        ///     The create instance and unwrap.
        /// </summary>
        /// <param name="domain">
        ///     The domain.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue CreateInstanceAndUnwrap<TValue>([NotNull] this AppDomain domain) where TValue : class
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            var targetType = typeof(TValue);
            return (TValue) domain.CreateInstanceAndUnwrap(targetType.Assembly.FullName, targetType.FullName);
        }

        /// <summary>
        ///     The create instance and unwrap.
        /// </summary>
        /// <param name="domain">
        ///     The domain.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        [NotNull]
        public static TValue CreateInstanceAndUnwrap<TValue>([NotNull] this AppDomain domain, [NotNull] params object[] args)
            where TValue : class
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            if (args == null) throw new ArgumentNullException(nameof(args));

            var targetType = typeof(TValue);
            return
                (TValue)
                domain.CreateInstanceAndUnwrap(
                    targetType.Assembly.FullName,
                    targetType.FullName,
                    false,
                    BindingFlags.Default,
                    null,
                    args,
                    null,
                    null);
        }

        /// <summary>
        ///     The find member attributes.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="nonPublic">
        ///     The non public.
        /// </param>
        /// <param name="bindingflags">
        ///     The bindingflags.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<Tuple<MemberInfo, TAttribute>> FindMemberAttributes<TAttribute>(
            [NotNull] this Type type,
            bool nonPublic,
            BindingFlags bindingflags) where TAttribute : Attribute
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            bindingflags |= BindingFlags.Public;
            if (nonPublic) bindingflags |= BindingFlags.NonPublic;

            if (!Enum.IsDefined(typeof(BindingFlags), BindingFlags.FlattenHierarchy))
                return from mem in type.GetMembers(bindingflags)
                    let attr = CustomAttributeExtensions.GetCustomAttribute<TAttribute>(mem)
                    where attr != null
                    select Tuple.Create(mem, attr);

            return from mem in type.GetHieratichialMembers(bindingflags)
                let attr = mem.GetCustomAttribute<TAttribute>()
                where attr != null
                select Tuple.Create(mem, attr);
        }

        [NotNull]
        public static IEnumerable<MemberInfo> GetHieratichialMembers([NotNull] this Type type, BindingFlags flags)
        {
            var targetType = type;
            while (targetType != null)
            {
                foreach (var mem in targetType.GetMembers(flags)) yield return mem;

                targetType = targetType.BaseType;
            }
        }

        /// <summary>
        ///     The find member attributes.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="nonPublic">
        ///     The non public.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<Tuple<MemberInfo, TAttribute>> FindMemberAttributes<TAttribute>([NotNull] this Type type,
            bool nonPublic) where TAttribute : Attribute
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return FindMemberAttributes<TAttribute>(
                type,
                nonPublic,
                BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        [NotNull]
        [System.Diagnostics.Contracts.Pure]
        public static T[] GetAllCustomAttributes<T>([NotNull] this ICustomAttributeProvider member) where T : Attribute
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            return (T[]) member.GetCustomAttributes(typeof(T), true);
        }

        [NotNull]
        [System.Diagnostics.Contracts.Pure]
        public static object[] GetAllCustomAttributes([NotNull] this ICustomAttributeProvider member, [NotNull] Type type)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return member.GetCustomAttributes(type, true);
        }

        /// <summary>
        ///     The get custom attribute.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TAttribute" />.
        /// </returns>
        [CanBeNull]
        public static TAttribute GetCustomAttribute<TAttribute>([NotNull] this ICustomAttributeProvider provider)
            where TAttribute : Attribute
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            return GetCustomAttribute<TAttribute>(provider, true);
        }

        /// <summary>
        ///     The get custom attribute.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <param name="inherit">
        ///     The inherit.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TAttribute" />.
        /// </returns>
        [CanBeNull]
        public static TAttribute GetCustomAttribute<TAttribute>([NotNull] this ICustomAttributeProvider provider, bool inherit)
            where TAttribute : Attribute
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            var temp = provider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();

            return (TAttribute) temp;
        }

        /// <summary>
        ///     The get custom attributes.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        /// <param name="attributeTypes">
        ///     The attribute types.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull]
        public static IEnumerable<object> GetCustomAttributes([NotNull] this ICustomAttributeProvider provider, [NotNull] [ItemNotNull] params Type[] attributeTypes)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            return attributeTypes.SelectMany(attributeType => provider.GetCustomAttributes(attributeType, false));
        }

        /// <summary>
        ///     The get invoke member.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        public static TType GetInvokeMember<TType>([NotNull] this MemberInfo info, [NotNull] object instance, [CanBeNull] params object[] parameter)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            try
            {
                if (info is PropertyInfo)
                {
                    var property = info.CastObj<PropertyInfo>();
                    if (parameter != null && parameter.Length == 0) parameter = null;

                    return (TType) property.GetValue(instance, parameter);
                }

                if (info is FieldInfo) return (TType) info.CastObj<FieldInfo>().GetValue(instance);

                if (info is MethodBase) return (TType) info.CastObj<MethodBase>().Invoke(instance, parameter);
            }
            catch (InvalidCastException)
            {
            }

            return default(TType);
        }

        /// <summary>
        ///     The get method handle.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <returns>
        ///     The <see cref="RuntimeMethodHandle" />.
        /// </returns>
        public static RuntimeMethodHandle GetMethodHandle([NotNull] this MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            var mi = method as MethodInfo;

            if (mi != null && mi.IsGenericMethod) return mi.GetGenericMethodDefinition().MethodHandle;

            return method.MethodHandle;
        }

        /// <summary>
        ///     The get parameter types.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull]
        public static IEnumerable<Type> GetParameterTypes([NotNull] this MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return method.GetParameters().Select(p => p.ParameterType);
        }

        /// <summary>
        ///     The get property from method.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="implementingType">
        ///     The implementing type.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo" />.
        /// </returns>
        [CanBeNull]
        public static PropertyInfo GetPropertyFromMethod([NotNull] this MethodInfo method, [NotNull] Type implementingType)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (implementingType == null) throw new ArgumentNullException(nameof(implementingType));
            if (!method.IsSpecialName || method.Name.Length < 4) return null;

            var isGetMethod = method.Name.Substring(0, 4) == "get_";
            var returnType = isGetMethod ? method.ReturnType : method.GetParameterTypes().Last();
            var indexerTypes = isGetMethod
                ? method.GetParameterTypes()
                : method.GetParameterTypes().SkipLast(1);

            return implementingType.GetProperty(
                method.Name.Substring(4),
                DefaultBindingFlags,
                null,
                returnType,
                indexerTypes.ToArray(),
                null);
        }

        /// <summary>
        ///     The get property from method.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo" />.
        /// </returns>
        [CanBeNull]
        public static PropertyInfo GetPropertyFromMethod([NotNull] this MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return !method.IsSpecialName ? null : method.DeclaringType.GetProperty(method.Name.Substring(4), DefaultBindingFlags);
        }

        /// <summary>
        ///     The get set invoke type.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        [NotNull]
        public static Type GetSetInvokeType([NotNull] this MemberInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo) info).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo) info).GetParameterTypes().First();
                case MemberTypes.Property:
                    return ((PropertyInfo) info).PropertyType;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     The has attribute.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasAttribute<T>([NotNull] this ICustomAttributeProvider member) where T : Attribute
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            return member.IsDefined(typeof(T), true);
        }

        /// <summary>
        ///     The has attribute.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasAttribute([NotNull] this ICustomAttributeProvider member, [NotNull] Type type)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (type == null) throw new ArgumentNullException(nameof(type));
            return member.IsDefined(type, true);
        }

        /// <summary>
        ///     The has matching attribute.
        /// </summary>
        /// <param name="member">
        ///     The member.
        /// </param>
        /// <param name="attributeToMatch">
        ///     The attribute to match.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public static bool HasMatchingAttribute<T>([NotNull] this ICustomAttributeProvider member, [NotNull] T attributeToMatch)
            where T : Attribute
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (attributeToMatch == null) throw new ArgumentNullException(nameof(attributeToMatch));
            var attributes = member.GetAllCustomAttributes<T>();

            return attributes.Length != 0 && attributes.Any(attribute => attribute.Match(attributeToMatch));
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        public static TType Invoke<TType>([NotNull] this MethodBase method, [NotNull] object instance, [NotNull] params object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            return (TType) method.Invoke(instance, args);
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        public static void Invoke([NotNull] this MethodBase method, [NotNull] object instance, [NotNull] params object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            method.Invoke(instance, args);
        }

        /// <summary>
        ///     The parse enum.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="TEnum">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEnum" />.
        /// </returns>
        public static TEnum ParseEnum<TEnum>([NotNull] this string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return (TEnum) Enum.Parse(typeof(TEnum), value);
        }

        public static TEnum TryParseEnum<TEnum>(this string value, TEnum defaultValue)
            where TEnum : struct 
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return defaultValue;

                return Enum.TryParse<TEnum>(value, out var e) ? e : defaultValue;
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///     The set invoke member.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public static void SetInvokeMember([NotNull] this MemberInfo info, [NotNull] object instance, [CanBeNull] params object[] parameter)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (info is PropertyInfo)
            {
                var property = info.CastObj<PropertyInfo>();
                object value = null;
                object[] indexes = null;
                if (parameter != null)
                {
                    if (parameter.Length >= 1) value = parameter[0];

                    if (parameter.Length > 1) indexes = parameter.Skip(1).ToArray();
                }

                property.SetValue(instance, value, indexes);
            }
            else if (info is FieldInfo)
            {
                object value = null;
                if (parameter != null) value = parameter.FirstOrDefault();

                info.CastObj<FieldInfo>().SetValue(instance, value);
            }
            else if (info is MethodBase)
            {
                info.CastObj<MethodBase>().Invoke(instance, parameter);
            }
        }

        /// <summary>
        ///     The parse enum.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="eEnum">
        ///     The e Enum.
        /// </param>
        /// <typeparam name="TEnum">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEnum" />.
        /// </returns>
        public static bool TryParseEnum<TEnum>([NotNull] this string value, out TEnum eEnum) where TEnum : struct
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return Enum.TryParse(value, out eEnum);
        }

        #endregion
    }
}