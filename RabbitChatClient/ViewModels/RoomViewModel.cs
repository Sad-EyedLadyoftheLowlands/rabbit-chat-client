using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using DynamicData.Tests;
using RabbitChatClient.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class RoomViewModel : ViewModelBase
{
    private string _text;

    private RabbitMqService _mqService;

    // private readonly IRabbitMqService _mqService;

    public ObservableCollection<string> Messages { get; } = new();

    public string Text
    {
        get => _text;
        set
        {
            Console.WriteLine($"Setting text to: {value}");
            this.RaiseAndSetIfChanged(ref _text, value);
        }
    }

    public RoomViewModel() // IRabbitMqService mqService
    {
        var t = new Thread(TestMq);
        t.Start();
        
        /*
        this.WhenAnyValue(x => test.Messages)
            .Subscribe(async x => Console.WriteLine(x));
        */
        /*
        this.WhenAnyValue(x => RabbitMqService.Integers)
            .Subscribe(async x => Test());
            */
        // _mqService = mqService;
        // _mqService.Test();

        
        // test.Test();
        TempGetMessages();
        /*
         * this.WhenAnyValue(x => x.SelectedFriendIndex).Subscribe(async x =>
        {
            Console.WriteLine($"Value from SelectedFriend: {x}");
            if (x >= 0 && x < Friends.Count)
                TriggerShowRoomDialog();
        });
         */
    }

    private void TempGetMessages()
    {
        for (var i = 0; i < 20; i++)
        {
            Messages.Add($"Message #{i}");
        }
    }

    /*
    private void Testing()
    {
        this.WhenAnyValue(x => _mqService.Messages)
            .Subscribe(async x => Console.WriteLine(x[0]));
    }
    */
    
    private void Test()
    {
        Console.WriteLine("HONK HONK!");
    }

    private void TestMq()
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
                // Messages.Clear();
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