using RabbitChatClient.Models;

namespace RabbitChatClient.ViewModels;

public class MessageViewModel
{
    private readonly Message _message;

    public string MessageContent => _message.MessageContent;

    public string MessageTime => _message.TimeSent.ToShortTimeString();

    public MessageViewModel(Message message)
    {
        _message = message;
    }
}