namespace Tauron.MgiManager.User.Shared.Dtos
{
    public sealed class UserValidationResult
    {
        public bool Validated { get; set; }

        public string Token { get; set; }

        public string Error { get; set; }

        public UserValidationResult()
        {
            
        }

        public UserValidationResult(bool validated, string token, string error)
        {
            Validated = validated;
            Token = token;
            Error = error;
        }
    }
}