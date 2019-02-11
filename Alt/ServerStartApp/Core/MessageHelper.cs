using System;
using System.Windows.Forms;

namespace ServerStartApp.Core
{
    public static class MessageHelper
    {
        private static RichTextBox _log;

        public static void Initialize(RichTextBox log) => _log = log;

        public static void SendMessage(string message)
        {
            if (_log?.InvokeRequired ?? true)
                _log?.Invoke(new Action<string>(SendMessage), message);

            if(_log == null) return;

            _log.Text += message + Environment.NewLine;
        }
    }
}