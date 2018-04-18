#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The control target attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [DebuggerNonUserCode]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public sealed class ControlTargetAttribute : MemberInfoAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ControlTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ControlTargetAttribute" /> Klasse.
        /// </summary>
        public ControlTargetAttribute()
            : base(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ControlTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ControlTargetAttribute" /> Klasse.
        /// </summary>
        /// <param name="memberName">
        ///     The member name.
        /// </param>
        public ControlTargetAttribute([CanBeNull] string memberName)
            : base(memberName)
        {
        }

        #endregion
    }

    /// <summary>The window target attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [DebuggerNonUserCode]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public sealed class WindowTargetAttribute : MemberInfoAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WindowTargetAttribute" /> Klasse.
        /// </summary>
        public WindowTargetAttribute()
            : base(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WindowTargetAttribute" /> Klasse.
        /// </summary>
        /// <param name="memberName">
        ///     The member name.
        /// </param>
        public WindowTargetAttribute([CanBeNull] string memberName)
            : base(memberName)
        {
        }

        #endregion
    }

    // [DebuggerNonUserCode]
    /// <summary>The control helper.</summary>
    [PublicAPI]
    public static class ControlHelper
    {
        [DebuggerNonUserCode]
        private class ControlLinker : LinkerBase
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ControlLinker" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="ControlLinker" /> Klasse.
            /// </summary>
            /// <param name="name">
            ///     The name.
            /// </param>
            /// <param name="element">
            ///     The element.
            /// </param>
            public ControlLinker([NotNull] string name, [NotNull] DependencyObject element)
                : base(name, element)
            {
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The scan.</summary>
            public override void Scan()
            {
                var context = DataContext == null ? null : DataContext.Target;
                if (context == null) return;

                MemberInfoAttribute.InvokeMembers<ControlTargetAttribute>(context, Name, Target);
            }

            #endregion
        }

        // [DebuggerNonUserCode]
        private abstract class LinkerBase : PipelineBase
        {
            #region Fields

            private string _name;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LinkerBase" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="LinkerBase" /> Klasse.
            /// </summary>
            /// <param name="name">
            ///     The name.
            /// </param>
            /// <param name="element">
            ///     The element.
            /// </param>
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            protected LinkerBase([NotNull] string name, [NotNull] DependencyObject element)
                : base(element, false)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (element == null) throw new ArgumentNullException(nameof(element));
                Name = name;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the name.</summary>
            [NotNull]
            public string Name
            {
                get => _name;

                set
                {
                    _name = value;
                    Scan();
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The scan.</summary>
            public abstract void Scan();

            #endregion

            #region Methods

            /// <summary>The data context changed.</summary>
            protected override void DataContextChanged()
            {
                Scan();
            }

            #endregion
        }

        // [DebuggerNonUserCode]
        private class WindowLinker : LinkerBase
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="WindowLinker" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="WindowLinker" /> Klasse.
            /// </summary>
            /// <param name="name">
            ///     The name.
            /// </param>
            /// <param name="element">
            ///     The element.
            /// </param>
            public WindowLinker([NotNull] string name, [NotNull] DependencyObject element)
                : base(name, element)
            {
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The scan.</summary>
            public override void Scan()
            {
                var    realName   = Name;
                string windowName = null;

                if (realName.Contains(":"))
                {
                    var nameSplit = realName.Split(new[] {':'}, 2);
                    realName   = nameSplit[0];
                    windowName = nameSplit[1];
                }

                object context;
                var    priTarget = Target;
                if (DataContext == null || (context = DataContext.Target) == null || priTarget == null)
                {
                    CommonConstants.LogCommon(false, "ControlHelper: No Context Found");
                    return;
                }

                if (windowName == null)
                {
                    if (!(priTarget is Window)) priTarget = Window.GetWindow(priTarget);

                    if (priTarget == null)
                        CommonWpfConstans.LogCommon(false, "ControlHelper: No Window Found: {0}|{1}", context.GetType(), realName);
                }
                else
                {
                    priTarget =
                        System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault(win => win.Name == windowName);

                    if (priTarget == null)
                        CommonWpfConstans.LogCommon(false, "ControlHelper: No Window Named {0} Found", windowName);
                }

                if (priTarget == null) return;

                foreach (var member in
                    MemberInfoAttribute.GetMembers<WindowTargetAttribute>(context.GetType())
                                       .Where(mem => mem.Item1 == realName))
                {
                    try
                    {
                        var targetType = member.Item2.GetSetInvokeType();

                        object arg;
                        if (targetType == typeof(IWindow)) arg = new WpfWindow((Window) priTarget);
                        else arg                               = priTarget;

                        member.Item2.SetInvokeMember(context, arg);
                    }
                    catch (Exception e)
                    {
                        CommonConstants.LogCommon(true, "ControlHelper: Error On {0} Member Acess: {1}", member.Item2.Name, e);

                        throw;
                    }
                }
            }

            #endregion
        }

        #region Static Fields

        public static readonly DependencyProperty MarkControlProperty =
            DependencyProperty.RegisterAttached(
                                                "MarkControl",
                                                typeof(string),
                                                typeof(ControlHelper),
                                                new UIPropertyMetadata(string.Empty, MarkControl));

        public static readonly DependencyProperty MarkWindowProperty = DependencyProperty.RegisterAttached(
                                                                                                           "MarkWindow",
                                                                                                           typeof(
                                                                                                               string),
                                                                                                           typeof(
                                                                                                                   ControlHelper
                                                                                                               ),
                                                                                                           new UIPropertyMetadata
                                                                                                               (null,
                                                                                                                MarkWindowChanged));

        private static readonly WeakReferenceCollection<LinkerBase> LinkerCollection =
            new WeakReferenceCollection<LinkerBase>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get mark control.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetMarkControl([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return (string) obj.GetValue(MarkControlProperty);
        }

        /// <summary>
        ///     The get mark window.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetMarkWindow([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return (string) obj.GetValue(MarkWindowProperty);
        }

        /// <summary>
        ///     The set mark control.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetMarkControl([NotNull] DependencyObject obj, [NotNull] string value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            obj.SetValue(MarkControlProperty, value);
        }

        /// <summary>
        ///     The set mark window.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetMarkWindow([NotNull] DependencyObject obj, [NotNull] string value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            obj.SetValue(MarkWindowProperty, value);
        }

        #endregion

        #region Methods

        private static void MarkControl([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetLinker(d, e.OldValue.As<string>() ?? throw new InvalidOperationException(), e.NewValue.As<string>() ?? throw new InvalidOperationException(), (obj, str) => new ControlLinker(str, obj));
        }

        // Using a DependencyProperty as the backing store for MarkWindow.  This enables animation, styling, binding, etc...
        private static void MarkWindowChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetLinker(d, e.OldValue.As<string>(), e.NewValue.As<string>(), (obj, str) => new WindowLinker(str, obj));
        }

        private static void SetLinker([NotNull] DependencyObject obj, [CanBeNull] string oldName, [CanBeNull] string newName, [NotNull] Func<DependencyObject, string, LinkerBase> factory)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (DesignerProperties.GetIsInDesignMode(obj)) return;

            foreach (var linker in
                LinkerCollection.Where(linker => Equals(linker.Target, obj) && linker.Name == oldName))
            {
                linker.Name = newName;
                return;
            }

            var pipline = factory(obj, newName);
            pipline.Scan();
            LinkerCollection.Add(pipline);
        }

        #endregion
    }
}