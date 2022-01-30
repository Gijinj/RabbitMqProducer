using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    internal class Topic
    {
        private const string _exchange = "my-topic-exchange";

        public static void Produce()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_exchange, ExchangeType.Topic);

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
            Console.Write("Enter topic name:");

            string? messageType = Console.ReadLine();

            Console.Write("Enter message. press enter to quit:");

            string? message = Console.ReadLine();
            if(string.IsNullOrEmpty(message))
                {
                return (null, null);
            }

            return (message, messageType);
        }
    }
}
