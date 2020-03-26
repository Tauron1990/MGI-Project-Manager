namespace Tauron.Application.SimpleAuth.Data
{
    /// <summary>
    /// Enthält alle information um ein neues Password zu setzen
    /// </summary>
    public sealed class NewPasswordData
    {
        /// <summary>
        /// Das Alte Password
        /// </summary>
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// Das Neue Passwort das gesetzt werden soll
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }
}