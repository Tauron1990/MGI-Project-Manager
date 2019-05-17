namespace Tauron.CQRS.Common.Security
{
    public interface IApiValidator
    {
        bool Validate(string apiKey);
    }
}