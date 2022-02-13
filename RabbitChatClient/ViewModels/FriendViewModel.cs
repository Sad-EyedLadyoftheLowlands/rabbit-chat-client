using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using RabbitChatClient.Models;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class FriendViewModel : ViewModelBase
{
    private readonly Friend _friend;

    public string UserName => _friend.UserName;

    public FriendViewModel(Friend friend)
    {
        _friend = friend;
    }
}