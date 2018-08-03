#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The data context changing complete attribute.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    [DebuggerNonUserCode]
    [PublicAPI]
    public sealed class DataContextChangingCompleteAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataContextChangingCompleteAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="DataContextChangingCompleteAttribute" /> Klasse.
        /// </summary>
        public DataContextChangingCompleteAttribute()
        {
            Sync = true;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether sync.</summary>
        public bool Sync { get; set; }

        #endregion
    }

    [PublicAPI]
    internal interface IPipeLine : IWeakReference
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The generate.
        /// </summary>
        /// <param name="dataContext">
        ///     The data context.
        /// </param>
        /// <param name="scheduler">
        ///     The scheduler.
        /// </param>
        /// <returns>
        ///     The <see cref="ITask" />.
        /// </returns>
        [NotNull]
        ITask Generate([NotNull] WeakReference dataContext, [NotNull] ITaskScheduler scheduler);

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the data context.</summary>
        [CanBeNull]
        WeakReference DataContext { get; set; }

        /// <summary>Gets or sets the task scheduler.</summary>
        [CanBeNull]
        ITaskScheduler TaskScheduler { get; set; }

        #endregion
    }

    // [DebuggerNonUserCode]
    /// <summary>The data context services.</summary>
    [PublicAPI]
    public static class DataContextServices
    {
        // [DebuggerNonUserCode]
        private class ObjectReference : IWeakReference
        {
            private class DataChangingCompledTask : ITask
            {
                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the <see cref="DataChangingCompledTask" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="DataChangingCompledTask" /> Klasse.
                /// </summary>
                /// <param name="info">
                ///     The info.
                /// </param>
                /// <param name="dataContext">
                ///     The data context.
                /// </param>
                /// <param name="sync">
                ///     The sync.
                /// </param>
                public DataChangingCompledTask([NotNull] MethodInfo info, [NotNull] object dataContext, bool sync)
                {
                    Synchronize  = sync;
                    _info        = info;
                    _dataContext = dataContext;
                    _task        = new TaskCompletionSource<object>();
                    _task.SetResult(null);
                }

                #endregion

                #region Public Methods and Operators

                /// <summary>The execute.</summary>
                public void Execute()
                {
                    _info.Invoke(_dataContext);
                }

                #endregion

                #region Fields

                private readonly object _dataContext;

                private readonly MethodInfo _info;

                private readonly TaskCompletionSource<object> _task;

                #endregion

                #region Public Properties

                /// <summary>Gets a value indicating whether synchronize.</summary>
                public bool Synchronize { get; }

                /// <summary>Gets the task.</summary>
                public Task Task => _task.Task;

                #endregion
            }

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ObjectReference" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="ObjectReference" /> Klasse.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            public ObjectReference([NotNull] DependencyObject obj)
            {
                _pips   = new List<IPipeLine>();
                _target = new WeakReference<DependencyObject>(obj);
            }

            #endregion

            #region Fields

            private readonly List<IPipeLine> _pips;

            private readonly WeakReference<DependencyObject> _target;

            #endregion

            #region Public Properties

            /// <summary>Gets the dependency object.</summary>
            [CanBeNull]
            public DependencyObject DependencyObject => _target.TypedTarget();

            /// <summary>Gets a value indicating whether is alive.</summary>
            public bool IsAlive => _target.IsAlive();

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The add pipline.
            /// </summary>
            /// <param name="pipline">
            ///     The pipline.
            /// </param>
            /// <param name="schedule">
            ///     The schedule.
            /// </param>
            public void AddPipline([NotNull] IPipeLine pipline, [NotNull] ITaskScheduler schedule)
            {
                var context = FindDataContext(DependencyObject);
                if (context != null)
                {
                    pipline.DataContext   = new WeakReference(context);
                    pipline.TaskScheduler = schedule;
                }

                if (_pips.Contains(pipline)) return;

                _pips.Add(pipline);
            }

            public void RemovePipline(IPipeLine pipeline)
            {
                _pips.Remove(pipeline);
            }

            /// <summary>
            ///     The is match.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool IsMatch([CanBeNull] object obj)
            {
                return ReferenceEquals(_target.TypedTarget(), obj);
            }

            /// <summary>
            ///     The new data context.
            /// </summary>
            /// <param name="dataContext">
            ///     The data context.
            /// </param>
            /// <param name="scheduler">
            ///     The scheduler.
            /// </param>
            public void NewDataContext([CanBeNull] object dataContext, [NotNull] ITaskScheduler scheduler)
            {
                var weakDataContext = new WeakReference(dataContext);
                foreach (var pip in _pips.ToArray()) scheduler.QueueTask(pip.Generate(weakDataContext, scheduler));

                var type = dataContext?.GetType();

                var temp =
                    type?.FindMemberAttributes<DataContextChangingCompleteAttribute>(true);

                var memInfo = temp?.FirstOrDefault();
                if (memInfo == null) return;

                scheduler.QueueTask(
                                    new DataChangingCompledTask(memInfo.Item1.CastObj<MethodInfo>(), dataContext,
                                                                memInfo.Item2.Sync));
            }

            #endregion
        }

        private class RequestingElement : IWeakReference
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="RequestingElement" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="RequestingElement" /> Klasse.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            /// <param name="pipline">
            ///     The pipline.
            /// </param>
            public RequestingElement([CanBeNull] DependencyObject obj, [NotNull] IPipeLine pipline)
            {
                _depObj  = new FrameworkObject(obj);
                _pipline = pipline;

                _depObj.LoadedEvent += LoadedEventHandler;
            }

            #endregion

            #region Explicit Interface Properties

            bool IWeakReference.IsAlive => !_isClear || ((IWeakReference) _depObj).IsAlive;

            #endregion

            #region Methods

            private void LoadedEventHandler([NotNull] object sender, [NotNull] RoutedEventArgs e)
            {
                _depObj.LoadedEvent -= LoadedEventHandler;
                _isClear            =  true;
                if (RegisterHandler(_depObj.Original, _pipline)) TriggerRebind(_depObj.Original, _pipline);
            }

            #endregion

            #region Fields

            private readonly FrameworkObject _depObj;

            private readonly IPipeLine _pipline;

            private bool _isClear;

            #endregion
        }

        #region Static Fields

        public static readonly DependencyProperty ActivateProperty = DependencyProperty.RegisterAttached(
                                                                                                         "Activate",
                                                                                                         typeof(bool),
                                                                                                         typeof(
                                                                                                                 DataContextServices
                                                                                                             ),
                                                                                                         new UIPropertyMetadata
                                                                                                             (false,
                                                                                                              OnActivateChanged));

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.RegisterAttached(
                                                "DataContext",
                                                typeof(object),
                                                typeof(DataContextServices),
                                                new FrameworkPropertyMetadata(null,
                                                                              FrameworkPropertyMetadataOptions.Inherits,
                                                                              BeginDataContextChanging));

        private static readonly WeakReferenceCollection<ObjectReference> Objects =
            new WeakReferenceCollection<ObjectReference>();

        private static readonly WeakReferenceCollection<RequestingElement> RequestingCollection =
            new WeakReferenceCollection<RequestingElement>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get activate.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool GetActivate([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return (bool) obj.GetValue(ActivateProperty);
        }

        /// <summary>
        ///     The get data context.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        public static object GetDataContext([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.GetValue(DataContextProperty);
        }

        /// <summary>
        ///     The set activate.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetActivate([NotNull] DependencyObject obj, bool value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.SetValue(ActivateProperty, value);
        }

        /// <summary>
        ///     The set data context.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetDataContext([NotNull] DependencyObject obj, [CanBeNull] object value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.SetValue(ActivateProperty, value);
        }

        /// <summary>
        ///     The trigger rebind.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        public static void TriggerRebind([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var objRef = FindObjectRecusiv(obj);

            var depObj = objRef?.DependencyObject;
            if (depObj == null) return;

            objRef.NewDataContext(FindDataContext(obj), CommonApplication.Scheduler);
        }

        #endregion

        #region Methods

        internal static void Activate([NotNull] DependencyObject element)
        {
            var objRef = FindObject(element);
            if (objRef != null) return;

            objRef = new ObjectReference(element);
            Objects.Add(objRef);

            new FrameworkObject(element, false).DataContextChanged += BeginDataContextChanging;
        }

        internal static void Deactivate([NotNull] DependencyObject element)
        {
            var objRef = FindObject(element);
            if (objRef == null) return;

            Objects.Remove(objRef);

            new FrameworkObject(element).DataContextChanged -= BeginDataContextChanging;
        }

        [CanBeNull]
        internal static object FindDataContext([CanBeNull] DependencyObject obj)
        {
            if (obj == null) return null;

            var result = new FrameworkObject(obj, false).DataContext ?? obj.GetValue(DataContextProperty);

            return result;
        }

        internal static bool RegisterHandler([CanBeNull] DependencyObject element, [NotNull] IPipeLine pipline)
        {
            var objRef = FindObjectRecusiv(element);

            if (objRef != null)
            {
                RegisterCore(objRef, pipline);
                return true;
            }

            RegisterForRequesting(element, pipline);
            return false;
        }

        internal static void UnregisterHandler([CanBeNull] DependencyObject element, [NotNull] IPipeLine pipeLine)
        {
            var objRef = FindObjectRecusiv(element);

            objRef?.RemovePipline(pipeLine);
        }

        private static void BeginDataContextChanging([NotNull] object d, DependencyPropertyChangedEventArgs e)
        {
            var objRef = FindObject(d);

            objRef?.NewDataContext(e.NewValue, CommonApplication.Scheduler);
        }

        [CanBeNull]
        [DebuggerStepThrough]
        private static ObjectReference FindObject([CanBeNull] object obj)
        {
            return Objects.FirstOrDefault(@ref => @ref.IsMatch(obj));
        }

        [CanBeNull]
        private static ObjectReference FindObjectRecusiv([CanBeNull] DependencyObject element)
        {
            ObjectReference objRef;

            do
            {
                objRef = FindObject(element);
                if (objRef != null) break;

                var temp = new FrameworkObject(element, false);
                element = temp.Parent ?? temp.VisualParent;
            } while (element != null);

            return objRef;
        }

        private static void OnActivateChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var after  = e.NewValue.CastObj<bool>();
            var before = e.OldValue.CastObj<bool>();
            if (after && before) return;

            if (before) Deactivate(d);

            if (!before && after) Activate(d);
        }

        private static void RegisterCore([NotNull] ObjectReference reference, [NotNull] IPipeLine pipline)
        {
            reference.AddPipline(pipline, CommonApplication.Scheduler);
        }

        private static void RegisterForRequesting([CanBeNull] DependencyObject obj, [NotNull] IPipeLine pipline)
        {
            RequestingCollection.Add(new RequestingElement(obj, pipline));
        }

        private static void TriggerRebind([CanBeNull] DependencyObject obj, [NotNull] IPipeLine pipline)
        {
            var refernence = FindObjectRecusiv(obj);
            if (refernence == null) return;

            var dataContext = FindDataContext(obj);
            if (dataContext == null) return;

            CommonApplication.Scheduler.QueueTask(
                                                  pipline.Generate(new WeakReference(dataContext),
                                                                   CommonApplication.Scheduler));
        }

        #endregion
    }
}