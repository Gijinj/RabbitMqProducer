using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Producer
{
    internal class RPC
    {
        public static void Call()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string replyQueueName = channel.QueueDeclare().QueueName;
                string correlationId = Guid.NewGuid().ToString();

                // reply
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] RPC reply received {0}", message);
                    }
                };

                channel.BasicConsume(queue: replyQueueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.Write("Enter name:");

                string? name = Console.ReadLine();

                while (!string.IsNullOrEmpty(name))
                {
                    var body = Encoding.UTF8.GetBytes(name);


                    var properties = channel.CreateBasicProperties();
                    properties.CorrelationId = correlationId;
                    properties.ReplyTo = replyQueueName;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "rpc_queue",
                                         basicProperties: properties,
                                         body: body);

                    Console.Write("Enter name:");
                    name = Console.ReadLine();
                }


                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
