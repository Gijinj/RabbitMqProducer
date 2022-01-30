using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    internal class Routing
    {
        private const string _exchange = "my-routing-exchange";

        public static void Produce()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_exchange, ExchangeType.Direct);

                string? message, messageType;
                (message, messageType)= GetMessage();

                while (!string.IsNullOrEmpty(message))
                {
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();

                    channel.BasicPublish(exchange: _exchange,
                                         routingKey: messageType,
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);

                    (message, messageType) = GetMessage();
                }
            }
        }

        private static (string?, string?) GetMessage()
        {
            Console.Write("Enter message. press enter to quit:");

            string? message = Console.ReadLine();
            if(string.IsNullOrEmpty(message))
                {
                return (null, null);
            }

            Console.Write("Enter message type:");

            string? messageType = Console.ReadLine();
            return (message, messageType);
        }
    }
}
