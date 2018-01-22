#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The member info attribute.</summary>
    [PublicAPI]
    public abstract class MemberInfoAttribute : Attribute
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberInfoAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MemberInfoAttribute" /> Klasse. Initialisiert eine neue Instanz der
        ///     Klasse.
        /// </summary>
        /// <param name="memberName">
        ///     Der Name des Members.
        /// </param>
        protected MemberInfoAttribute([CanBeNull] string memberName)
        {
            MemberName = memberName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Der Name des Kommandos
        /// </summary>
        [CanBeNull]
        public string MemberName { get; }

        /// <summary>Gets or sets a value indicating whether synchronize.</summary>
        public bool Synchronize { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get members.
        /// </summary>
        /// <param name="targetType">
        ///     The target type.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<Tuple<string, MemberInfo>> GetMembers<TAttribute>([NotNull] Type targetType)
            where TAttribute : MemberInfoAttribute
        {
            return
                targetType.FindMemberAttributes<TAttribute>(true)
                    .Select(
                        attribute =>
                            Tuple.Create(attribute.Item2.ProvideMemberName(attribute.Item1), attribute.Item1));
        }

        /// <summary>
        ///     The invoke members.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <param name="targetMember">
        ///     The target member.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void InvokeMembers<TAttribute>([NotNull] object instance, [NotNull] string targetMember, [NotNull] params object[] parameters)
            where TAttribute : MemberInfoAttribute
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (string.IsNullOrEmpty(targetMember)) throw new ArgumentException("Value cannot be null or empty.", nameof(targetMember));
            foreach (var member in
                GetMembers<TAttribute>(instance.GetType()).Where(member => member.Item1 == targetMember)) member.Item2.SetInvokeMember(instance, parameters);
        }

        /// <summary>
        ///     The provide member name.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public virtual string ProvideMemberName([NotNull] MemberInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            return MemberName ?? info.Name;
        }

        #endregion
    }
}