namespace IISServiceManager.Contratcs
{
    public interface IWebService
    {
        string Name { get; }

        string Id { get; }

        string Description { get; }

        ServiceType ServiceType { get; }
    }
}