namespace Tauron.Application.Data.Raven
{
    public interface IDatabaseRoot
    {
        IDatabaseSession OpenSession(bool noTracking = true);
    }
}