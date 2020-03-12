namespace Tauron.Application.Data.Raven
{
    public interface IDatabaseCache
    {
        IDatabaseRoot Get(string databaseName);
    }
}