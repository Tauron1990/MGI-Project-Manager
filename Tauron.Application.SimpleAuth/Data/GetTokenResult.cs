namespace Tauron.Application.SimpleAuth.Data
{
    /// <summary>
    ///     Enthält bei erfolg das Token das Generiert wurde.
    /// </summary>
    public sealed class GetTokenResult : OperationResultBase<GetTokenResult>
    {
        /// <summary>
        ///     Das Token zur Authentifizierung
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}