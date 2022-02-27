using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Concurrency;
using System.Text;
using System.Text.Json;
using System.Threading;
using RabbitChatClient.Models;
using RabbitChatClient.Models.Requests;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class RoomViewModel : ViewModelBase
{
    private string _text;
    private string _mqMessage;
    private int _roomId;
    private int _selectedMessageIndex;
    private HttpClient _httpClient;

    // public ObservableCollection<string> Messages { get; } = new();

    // public ObservableCollection<Message> FullMessages { get; } = new();

    public ObservableCollection<MessageViewModel> Messages { get; } = new();

    public string MqMessage
    {
        get => _mqMessage;
        set
        {
            Console.WriteLine($"Received Rabbit Mq message: {value}");
            FetchMessages();
            this.RaiseAndSetIfChanged(ref _mqMessage, value);
        }
    }
    
    public string Text
    {
        get => _text;
        set
        {
            Console.WriteLine($"Setting text to: {value}");
            this.RaiseAndSetIfChanged(ref _text, value);
        }
    }

    public int SelectedMessageIndex
    {
        get => _selectedMessageIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMessageIndex, value);
        }
    }

    public RoomViewModel(HttpClient httpClient, int roomId)
    {
        _httpClient = httpClient;
        _roomId = roomId;
        
        // Listening for Rabbit Mq messages must not block UI thread.
        new Thread(ListenForMqMessages).Start();

        RxApp.MainThreadScheduler.Schedule(FetchMessages);
    }

    private async void FetchMessages()
    {
        // Make sure we do not have duplicates.
        Messages.Clear();
        
        var response = await _httpClient.GetAsync($"http://localhost:5000/api/message/{_roomId}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        var messages = JsonSerializer.Deserialize<List<Message>>(json, 
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        foreach (var message in messages)
        {
            Messages.Add(new MessageViewModel(message));
        }

        // Forces scrolling to most recent message.
        SelectedMessageIndex = Messages.Count - 1;
    }

    private void ListenForMqMessages()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("RabbitChat", ExchangeType.Fanout);

        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName,
            "RabbitChat",
            "");
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) => 
            MqMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
        channel.BasicConsume(queueName,
            true,
            consumer);

        // TODO: There must be a better way to listen indefinitely.
        Console.ReadLine();
    }

    public async void HandleSendMessageRequested()
    {
        Console.WriteLine("Sending message");

        // Send message
        var createMessageRequest = new CreateMessageRequest
        {
            // TODO: Hardcoded, but data exists in MainWindowsViewModel
            SendingUserId = 1,
            RoomId = _roomId,
            Content = Text
        };

        var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/message", createMessageRequest);

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStreamAsync();
        var test = JsonSerializer.Deserialize<bool>(json);

        // Clear text
        Text = string.Empty;

        // ???
    }
}