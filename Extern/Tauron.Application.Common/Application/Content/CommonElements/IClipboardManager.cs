#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ClipboardManager interface.</summary>
    public interface IClipboardManager
    {

        [NotNull]
        ClipboardViewer CreateViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization);
    }
}