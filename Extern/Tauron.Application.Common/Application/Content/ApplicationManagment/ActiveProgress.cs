using JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>The active progress.</summary>
    [PublicAPI]
    public class ActiveProgress
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActiveProgress" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ActiveProgress" /> Klasse.
        ///     Initializes a new instance of the <see cref="ActiveProgress" /> class.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="percent">
        ///     The percent.
        /// </param>
        /// <param name="overAllProgress">
        ///     The over all progress.
        /// </param>
        public ActiveProgress([NotNull] string message, double percent, double overAllProgress)
        {
            if (percent < 0) percent                 = 0;
            if (overAllProgress < 0) overAllProgress = 0;

            if (percent > 100 || double.IsNaN(percent)) percent = 100;

            if (overAllProgress > 100 || double.IsNaN(overAllProgress)) overAllProgress = 100;

            Message         = message;
            Percent         = percent;
            OverAllProgress = overAllProgress;
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets the message.</summary>
        /// <value>The message.</value>
        [NotNull]
        public string Message { get; private set; }

        /// <summary>Gets or sets the over all progress.</summary>
        /// <value>The over all progress.</value>
        public double OverAllProgress { get; set; }

        /// <summary>Gets the percent.</summary>
        /// <value>The percent.</value>
        public double Percent { get; private set; }

        #endregion
    }
}