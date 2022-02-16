using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class RoomViewModel : ViewModelBase
{
    private string _text;

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

    public RoomViewModel()
    {
        TempGetMessages();
    }

    private void TempGetMessages()
    {
        for (var i = 0; i < 20; i++)
        {
            Messages.Add($"Message #{i}");
        }
    }
}