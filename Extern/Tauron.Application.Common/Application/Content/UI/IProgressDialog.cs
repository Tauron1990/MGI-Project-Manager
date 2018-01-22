#region

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The progress style.</summary>
    [PublicAPI]
    public enum ProgressStyle
    {
        /// <summary>The none.</summary>
        None,

        /// <summary>The progress bar.</summary>
        ProgressBar,

        /// <summary>The marquee progress bar.</summary>
        MarqueeProgressBar
    }

    /// <summary>The ProgressDialog interface.</summary>
    [PublicAPI]
    public interface IProgressDialog : IDisposable
    {
        #region Public Properties

        /// <summary>Gets or sets the progress bar style.</summary>
        /// <value>The progress bar style.</value>
        ProgressStyle ProgressBarStyle { get; set; }

        #endregion

        #region Public Events

        /// <summary>The completed.</summary>
        event EventHandler Completed;

        #endregion

        #region Public Methods and Operators

        /// <summary>The start.</summary>
        void Start();

        #endregion
    }
}