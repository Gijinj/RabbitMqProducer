using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    internal class Broadcast
    {
        public static void Produce()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                Console.Write("Enter message. press enter to quit:");

                string? message = Console.ReadLine();

                while (!string.IsNullOrEmpty(message))
                {
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ExchangeDeclare("my-exchange", ExchangeType.Fanout);

                    channel.BasicPublish(exchange: "my-exchange",
                                         routingKey: "hello",
                                         basicProperties: properties,
                                         body: body);

                    Console.WriteLine(" [x] Sent {0}", message);

                    Console.Write("Enter message. press enter to quit:");
                    message = Console.ReadLine();
                }
            }
        }
    }
}
