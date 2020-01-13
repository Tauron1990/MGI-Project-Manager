namespace Tauron.Application.OptionsStore.Data
{
    public interface IDataClient
    {
        IOptionDataCollection GetCollection(string name);
    }
}