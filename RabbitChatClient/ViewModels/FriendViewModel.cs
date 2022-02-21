using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using RabbitChatClient.Models;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class FriendViewModel : ViewModelBase
{
    private readonly RabbitUser _friend;

    public string UserName => _friend.Username;

    public string Alias => _friend.Alias;

    public int FriendId => _friend.RabbitUserId;

    public FriendViewModel(RabbitUser friend)
    {
        _friend = friend;
    }
}