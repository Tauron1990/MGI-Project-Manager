#region

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The Task interface.</summary>
    public interface ITask
    {
        #region Public Methods and Operators

        /// <summary>The execute.</summary>
        void Execute();

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether synchronize.</summary>
        /// <value>The synchronize.</value>
        bool Synchronize { get; }

        /// <summary>Gets the task.</summary>
        [NotNull]
        Task Task { get; }

        #endregion
    }
}