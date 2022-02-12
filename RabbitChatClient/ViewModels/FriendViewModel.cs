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

    public ICommand OpenRoom { get; }
    
    public Interaction<RoomViewModel, Unit> ShowRoomDialog { get; }

    public FriendViewModel(Friend friend)
    {
        _friend = friend;

        /*
        ShowRoomDialog = new Interaction<RoomViewModel, Unit>();

        OpenRoom = ReactiveCommand.CreateFromTask(async () =>
        {
            var room = new RoomViewModel();
            var result = await ShowRoomDialog.Handle(room);
        });
        */
    }

    public void Test()
    {
        Console.WriteLine("In test");
    }

}