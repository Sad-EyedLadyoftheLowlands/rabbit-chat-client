using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using RabbitChatClient.Models;
using RabbitChatClient.Models.Requests;
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

        private int _connectedUserId = 1;

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

        public ObservableCollection<FriendViewModel> FriendViewModels { get; } = new();

        public ObservableCollection<RabbitUser> Friends { get; } = new();

        public ICommand BuyMusicCommand { get; }

        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

        public Interaction<RoomViewModel, string?> ShowRoomDialog { get; }

        public MainWindowViewModel() // IRabbitMqService mqService
        {
            _httpClient = new HttpClient();
            
            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

            ShowRoomDialog = new Interaction<RoomViewModel, string?>();

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
                // Confirm that value is within the range of Friends. 
                if (x >= 0 && x < FriendViewModels.Count)
                    TriggerShowRoomDialog();
            });

            // RxApp.MainThreadScheduler.Schedule(LoadAlbums);
            RxApp.MainThreadScheduler.Schedule(LoadFriends);
            // RxApp.MainThreadScheduler.Schedule(LoadFriendsDemo);
        }

        private async Task TriggerShowRoomDialog()
        {
            // Get friend from list by index and pass into new RoomViewModel.
            var friend = Friends.First(x => 
                x.RabbitUserId == FriendViewModels[SelectedFriendIndex].FriendId);
            
            // Get room.
            var request = new OpenPersonalRoomRequest
            {
                FriendId = _connectedUserId,
                RequestUserId = friend.RabbitUserId
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/room", request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            
            var room = JsonSerializer.Deserialize<Room>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            // Pass room into view model constructor.
            var roomViewModel = new RoomViewModel(_httpClient, room.RoomId);
            var result = await ShowRoomDialog.Handle(roomViewModel);

            Console.WriteLine("After result returned from ShowRoomDialog.Handle()");
            SelectedFriendIndex = -1;
        }

        private async Task<Room?> GetRoomId(int requestedFriendId)
        {
            var request = new OpenPersonalRoomRequest
            {
                FriendId = _connectedUserId,
                RequestUserId = requestedFriendId
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/room", request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            
            var room = JsonSerializer.Deserialize<Room>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return room;

            // var x = 5;
            // return await Task.FromResult(5);
        }
        
        private async void LoadFriends()
        {
            // TODO: Should come from settings.
            var response = await _httpClient.GetAsync("http://localhost:5000/api/user/getfriends/1");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var friends = JsonSerializer.Deserialize<List<RabbitUser>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            foreach (var friend in friends)
            {
                Friends.Add(friend);
                FriendViewModels.Add(new FriendViewModel(friend));
            }
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