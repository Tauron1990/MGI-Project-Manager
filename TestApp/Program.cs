using System;
using System.Threading.Tasks;
using RestEase;

namespace TestApp
{
    public interface IApiRequester
    {
        [Get(nameof(RegisterApiKey))]
        Task<string> RegisterApiKey(string serviceName);
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                const string URL = "http://localhost:81";

                var client = new RestClient(
                        new Uri(
                            new Uri(URL), "Api/ApiRequester"))
                   .For<IApiRequester>();

                var key = await client.RegisterApiKey("TestService");

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
