using System;
using System.Collections.ObjectModel;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitChatClient.Services;

/// <summary>
/// Interface for Rabbit MQ service to be implemented.
/// </summary>
public interface IRabbitMqService
{
    void Connect();
}

/// <summary>
/// Must eventually implement with DI as the Rabbit Mq service that exposes an observable message.
/// </summary>
public class RabbitMqService : IRabbitMqService
{
    public ObservableCollection<string> Messages { get; } = new();
    
    public RabbitMqService()
    {
        Connect();
    }

    public void Connect()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: "RabbitChat", type: ExchangeType.Fanout);

        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName,
            exchange: "RabbitChat",
            routingKey: "");

        Console.WriteLine(" [*] Waiting for logs.");
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] {0}", message);
            Messages.Clear();
            Messages.Add(message);
            foreach (var message1 in Messages)
            {
                Console.WriteLine(message1);
            }
        };
        channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);

        Console.ReadLine();
    }
}