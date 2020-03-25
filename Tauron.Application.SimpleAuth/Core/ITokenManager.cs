namespace Tauron.Application.SimpleAuth.Core
{
    public interface ITokenManager
    {
        string GenerateToken();
        bool ValidateToken(string token);
    }
}