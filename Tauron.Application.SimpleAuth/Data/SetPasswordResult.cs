namespace Tauron.Application.SimpleAuth.Data
{
    /// <summary>
    /// Ergebnis des Passowort-Setz vorgangas
    /// </summary>
    public sealed class SetPasswordResult : OperationResultBase<SetPasswordResult>
    {


        /// <summary>
        /// Ein neues token für die Authentifizierung
        /// </summary>
        public string Token { get; set; } = string.Empty;

        
    }
}