using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData.Binding;
using DynamicData.Tests;
using RabbitChatClient.Models;
using ReactiveUI;

namespace RabbitChatClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly HttpClient _httpClient;
        
        private bool _collectionEmpty;

        private string _selectedUser;

        private Friend _selectedFriend;

        private int _selectedFriendIndex = -1;

        public int SelectedFriendIndex
        {
            get => _selectedFriendIndex;
            set 
            {
                Console.WriteLine($"Selected friend index set to: {value}");
                this.RaiseAndSetIfChanged(ref _selectedFriendIndex, value);
            }
        }

        public Friend SelectedFriend
        {
            get => _selectedFriend;
            set
            {
                Console.WriteLine($"Selected friend changed to: {value}");
                this.RaiseAndSetIfChanged(ref _selectedFriend, value);
            }
        }

        public string SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }

        public bool CollectionEmpty
        {
            get => _collectionEmpty;
            set => this.RaiseAndSetIfChanged(ref _collectionEmpty, value);
        }

        public ObservableCollection<AlbumViewModel> Albums { get; } = new();

        public ObservableCollection<FriendViewModel> Friends { get; } = new();

        public ObservableCollection<string> Usernames { get; } = new();

        public ICommand BuyMusicCommand { get; }
        // public ICommand ShowRoom { get; }
        
        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

        public Interaction<RoomViewModel, string?> ShowRoomDialog { get; }

        public MainWindowViewModel()
        {
            _httpClient = new HttpClient();
            
            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

            ShowRoomDialog = new Interaction<RoomViewModel, string?>();

            /*
            ShowRoom = ReactiveCommand.CreateFromTask(async () =>
            {
                var room = new RoomViewModel();
                var result = await ShowRoomDialog.Handle(room);
            });
            */
            
            BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MusicStoreViewModel();
                var result = await ShowDialog.Handle(store);
                
                if (result != null)
                {
                    Console.WriteLine($"Album purchased from overlay: {result.Title}");
                    Albums.Add(result);

                    await result.SaveToDiskAsync();
                }
            });

            this.WhenAnyValue(x => x.Albums.Count)
                .Subscribe(x => CollectionEmpty = x == 0);

            this.WhenAnyValue(x => x.SelectedFriendIndex).Subscribe(async x =>
            {
                Console.WriteLine($"Value from SelectedFriend: {x}");
                if (x >= 0 && x < Friends.Count)
                    TriggerShowRoomDialog();
            });

            // RxApp.MainThreadScheduler.Schedule(LoadAlbums);
            // RxApp.MainThreadScheduler.Schedule(LoadFriendsDep);
            RxApp.MainThreadScheduler.Schedule(LoadFriends);
        }

        private async Task TriggerShowRoomDialog()
        {
            var room = new RoomViewModel();
            var result = await ShowRoomDialog.Handle(room);

            Console.WriteLine("After result returned from ShowRoomDialog.Handle()");
            SelectedFriendIndex = -1;
        }

        private void LoadFriends()
        {
            Friends.Add(new FriendViewModel(new Friend("First User")));
            Friends.Add(new FriendViewModel(new Friend("Second User")));
            Friends.Add(new FriendViewModel(new Friend("Third User")));
        }
        
        private async void LoadFriendsDep()
        {
            var response = await _httpClient.GetAsync("http://localhost:5000/api/user/getfriends/1");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var friends = JsonSerializer.Deserialize<List<RabbitUser>>(json);

            foreach (var friend in friends)
            {
                Usernames.Add(friend.username);
            }
            
            Console.WriteLine(friends);
        }
        
        private async void LoadAlbums()
        {
            var albums = (await Album.LoadCachedAsync()).Select(x => new AlbumViewModel(x));

            foreach (var album in albums)
            {
                Albums.Add(album);
            }

            foreach (var album in Albums.ToList())
            {
                await album.LoadCover();
            }
        }
    }
}