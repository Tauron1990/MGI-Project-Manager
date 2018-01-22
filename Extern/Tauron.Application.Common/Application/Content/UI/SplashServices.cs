#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The splash message listener.</summary>
    public sealed class SplashMessageListener : ObservableObject
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplashMessageListener" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SplashMessageListener" /> Klasse.
        /// </summary>
        public SplashMessageListener()
        {
            CurrentListner = this;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The receive message.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public void ReceiveMessage([NotNull] string message)
        {
            Message = message;
        }

        #endregion

        #region Fields

        /// <summary>The _message.</summary>
        private string _message;

        /// <summary>The _splash content.</summary>
        private object _splashContent;

        private double _height = 236;

        private object _mainLabelForeground = "Black";

        private object _mainLabelBackground = "White";

        private double _width = 414;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the current listner.</summary>
        [NotNull]
        public static SplashMessageListener CurrentListner { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        public double Height
        {
            get => _height;

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the main label foreground.
        /// </summary>
        [NotNull]
        public object MainLabelForeground
        {
            get => _mainLabelForeground;

            set
            {
                _mainLabelForeground = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the main labelbackground.
        /// </summary>
        [NotNull]
        public object MainLabelBackground
        {
            get => _mainLabelBackground;

            set
            {
                _mainLabelBackground = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Get or set received message.</summary>
        /// <value>The message.</value>
        [NotNull]
        public string Message
        {
            get => _message;

            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the splash content.</summary>
        /// <value>The splash content.</value>
        [NotNull]
        public object SplashContent
        {
            get => _splashContent;

            set
            {
                _splashContent = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the widht.
        /// </summary>
        public double Width
        {
            get => _width;

            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }

    /// <summary>The SplashService interface.</summary>
    [PublicAPI]
    public interface ISplashService
    {
        #region Public Properties

        /// <summary>Gets the listner.</summary>
        /// <value>The listner.</value>
        [NotNull]
        SplashMessageListener Listner { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The close splash.</summary>
        void CloseSplash();

        /// <summary>The show splash.</summary>
        void ShowSplash();

        #endregion
    }
}