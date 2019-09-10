using System;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string host = "amqp://192.168.178.43:5672";

            var test = EasyNetQ.RabbitHutch.CreateBus(host);

            test.Publish("Hallo World", configuration => configuration.WithQueueName("Test"));

            Console.WriteLine("Hello World!");
        }
    }
}
