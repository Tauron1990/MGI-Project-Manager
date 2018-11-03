using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Views;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public abstract class ViewModelBase : ModelBase, IShowInformation
    {
        private static bool? _isInDesignMode;

        private static IDialogFactory _dialogs;

        protected ViewModelBase()
        {
            ModelList = new Dictionary<string, ModelBase>();
        }

        public static bool IsInDesignMode
        {
            get
            {
                if (_isInDesignMode.HasValue) return _isInDesignMode.Value;
                var dependencyPropertyDescriptor =
                    DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                        typeof(FrameworkElement));
                _isInDesignMode = (bool) dependencyPropertyDescriptor.Metadata.DefaultValue;
                return _isInDesignMode.Value;
            }
        }

        [NotNull] protected Dictionary<string, ModelBase> ModelList { get; private set; }

        [NotNull] [Inject] public ViewManager ViewManager { get; protected set; }

        [NotNull]
        public static IDialogFactory Dialogs =>
            _dialogs ?? (_dialogs = CommonApplication.Current.Container.Resolve<IDialogFactory>());

        [NotNull] public System.Windows.Application CurrentApplication => System.Windows.Application.Current;

        [NotNull] public Dispatcher SystemDispatcher => CurrentApplication.Dispatcher;

        [CanBeNull] public static IWindow MainWindow => CommonApplication.Current.MainWindow;

        [NotNull] public IUISynchronize Synchronize => UiSynchronize.Synchronize;

        protected bool EditingInheritedModel { get; set; }

        protected override bool HasErrorOverride => ModelList.Values.Any(m => m.HasErrors);

        public virtual void OnShow(IWindow window)
        {
        }

        public virtual void AfterShow(IWindow window)
        {
        }

        [NotNull]
        public static ViewModelBase ResolveViewModel([NotNull] string name)
        {
            return CommonApplication.Current.Container.Resolve<ViewModelBase>(name, false);
        }

        protected internal void RegisterInheritedModel([NotNull] string name, [NotNull] ModelBase model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            model.PropertyChanged += ModelOnPropertyChanged;

            ModelList.Add(name, model);
        }

        protected virtual void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(this, propertyChangedEventArgs);
        }

        [NotNull]
        internal ModelBase GetModelBase([NotNull] string name)
        {
            return ModelList[name];
        }

        internal bool ContainsModel([NotNull] string name)
        {
            return ModelList.ContainsKey(name);
        }


        protected bool UnRegisterInheritedModel([NotNull] string model)
        {
            GetModelBase(model).PropertyChanged -= ModelOnPropertyChanged;
            return ModelList.Remove(model);
        }

        protected LinkedProperty LinkProperty(string name, INotifyPropertyChanged target, string customName = null)
        {
            return new LinkedProperty(this, name, target, customName);
        }

        protected LinkedProperty LinkPropertyExp<T>(Expression<Func<T>> name, INotifyPropertyChanged target,
            string customName = null)
        {
            return new LinkedProperty(this, PropertyHelper.ExtractPropertyName(name), target, customName);
        }

        protected override IEnumerable<ObservablePropertyDescriptor> CustomObservableProperties()
        {
            return ModelList.Values.SelectMany(m => m.GetPropertyDescriptors());
        }

        public override void BeginEdit()
        {
            if (EditingInheritedModel)
                foreach (var value in ModelList.Values)
                    value.BeginEdit();

            base.BeginEdit();
        }

        public override void EndEdit()
        {
            if (EditingInheritedModel)
                foreach (var value in ModelList.Values)
                    value.EndEdit();

            base.EndEdit();
        }

        public override void CancelEdit()
        {
            if (EditingInheritedModel)
                foreach (var value in ModelList.Values)
                    value.CancelEdit();

            base.CancelEdit();
        }

        protected override IEnumerable GetErrorsOverride(string property)
        {
            var first = ModelList.Values.FirstOrDefault(mb => mb.GetIssuesDictionary().ContainsKey(property));

            return first?.GetIssuesDictionary()[property];
        }

        protected void InvalidateRequerySuggested()
        {
            CommonApplication.Scheduler.QueueTask(
                new UserTask(() => CurrentDispatcher.BeginInvoke(CommandManager.InvalidateRequerySuggested), false));
        }

        protected override void OnErrorsChanged(string propertyName)
        {
            CommonApplication.Scheduler.QueueTask(
                new UserTask(() => { Synchronize.Invoke(() => base.OnErrorsChanged(propertyName)); }, false));
        }

        [PublicAPI]
        protected class LinkedProperty : IDisposable
        {
            private readonly string _custom;
            private ObservableObject _host;
            private string _name;
            private INotifyPropertyChanged _target;

            public LinkedProperty(ObservableObject host, string name, INotifyPropertyChanged target, string custom)
            {
                _host = host;
                _name = name;
                _target = target;
                _custom = custom;

                _target.PropertyChanged += PropertyChangedMethod;
            }

            public void Dispose()
            {
                Stop();
            }

            private void PropertyChangedMethod(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != _name) return;

                _host.OnPropertyChangedExplicit(_custom ?? _name);
            }

            public void Stop()
            {
                if (_target == null) return;

                _target.PropertyChanged -= PropertyChangedMethod;

                _host = null;
                _name = null;
                _target = null;
            }
        }

        [PublicAPI]
        protected class LinkedProperty : IDisposable
        {
            private readonly string _custom;
            private ObservableObject _host;
            private string _name;
            private INotifyPropertyChanged _target;

            public LinkedProperty(ObservableObject host, string name, INotifyPropertyChanged target, string custom)
            {
                _host = host;
                _name = name;
                _target = target;
                _custom = custom;

                _target.PropertyChanged += PropertyChangedMethod;
            }

            public void Dispose()
            {
                Stop();
            }

            private void PropertyChangedMethod(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != _name) return;

                _host.OnPropertyChangedExplicit(_custom ?? _name);
            }

            public void Stop()
            {
                if (_target == null) return;

                _target.PropertyChanged -= PropertyChangedMethod;

                _host = null;
                _name = null;
                _target = null;
            }
        }
    }
}