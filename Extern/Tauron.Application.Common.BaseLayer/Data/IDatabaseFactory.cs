namespace Tauron.Application.Common.BaseLayer.Data
{
    public interface IDatabaseFactory
    {
        string Id { get; }

        IDatabase CreateDatabase();
    }
}