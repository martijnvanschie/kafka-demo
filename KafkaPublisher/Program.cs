using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaPublisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };

            Action<DeliveryReport<string, string>> handler = r =>
                Console.WriteLine("Test");

            var producer = new ProducerBuilder<string, string>(config).Build();
            using (producer)
            {
                Console.WriteLine("\n-----------------------------------------------------------------------");
                Console.WriteLine($"Producer {producer.Name} producing on topic {"test"}.");
                Console.WriteLine("-----------------------------------------------------------------------");

                var stringValue = "";
                for (int i = 0; i < 100; i++)
                {
                    stringValue += "Y";

                    try
                    {
                        var deliveryReport = await producer.ProduceAsync("test", new Message<string, string> {Key = null, Value = stringValue });
                        Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }
                }

                producer.Flush(timeout: TimeSpan.FromSeconds(10));
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
