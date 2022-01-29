// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    string message = "Hello World!";
    var body = Encoding.UTF8.GetBytes(message);

    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    channel.BasicPublish(exchange: "",
                         routingKey: "hello",
                         basicProperties: properties,
                         body: body);
    Console.WriteLine(" [x] Sent {0}", message);
}

