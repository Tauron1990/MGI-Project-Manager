using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using JetBrains.Annotations;
using Tauron.Application.Commands;
using Tauron.Application.Models;

namespace Tauron.Application
{
    /// <summary>The tab workspace.</summary>
    public abstract class TabWorkspace : ViewModelBase, ITabWorkspace
    {
        #region Constants

        /// <summary>The close event name.</summary>
        protected const string CloseEventName = "CloseEvent";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TabWorkspace" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TabWorkspace" /> Klasse.
        /// </summary>
        /// <param name="title">
        ///     The title.
        /// </param>
        protected TabWorkspace([NotNull] string title, string name = null)
        {
            _title = title;
            _name = name;
            _canClose = true;
            CloseWorkspace = new SimpleCommand(obj => CanClose, obj => InvokeClose());
        }

        #endregion

        #region Public Events

        /// <summary>The close.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ITabWorkspace> Close;

        #endregion

        #region Public Methods and Operators

        /// <summary>The invoke close.</summary>
        public virtual void InvokeClose()
        {
            Close?.Invoke(this);
        }

        #endregion

        #region Fields

        private bool _canClose;

        private string _title;
        private readonly string _name;

        #endregion

        #region Public Properties
        
        public bool CanClose
        {
            get => _canClose;

            set
            {
                _canClose = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand CloseWorkspace { get; private set; }
        
        public string Title
        {
            get => _title;

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Name => _name ?? _title;

        public virtual void OnClose()
        {
        }

        public virtual void OnActivate()
        {
        }

        public virtual void OnDeactivate()
        {
        }

        #endregion
    }
}