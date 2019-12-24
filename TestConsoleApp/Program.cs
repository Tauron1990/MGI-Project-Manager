using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TestConsoleApp
{
    class Program
    {
        class Test
        {
            public string Eins { get; }

            public string Zwei { get; }

            public Test(string eins, string zwei)
            {
                Eins = eins;
                Zwei = zwei;
            }
        }

        static void Main(string[] args)
        {
            var test = new Test("Hallo", "Welt");

            var testText = JsonConvert.SerializeObject(test);

            test = JsonConvert.DeserializeObject<Test>(testText);
        }
    }
}
