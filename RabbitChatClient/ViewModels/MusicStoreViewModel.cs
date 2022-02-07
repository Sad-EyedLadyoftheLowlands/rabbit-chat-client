using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using RabbitChatClient.Models;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class MusicStoreViewModel : ViewModelBase
{
    private string? _searchText;
    private bool _isBusy;
    private AlbumViewModel? _selectedAlbum; // Why is this a ViewModel and not just a model? Because it is displayed. 
    private CancellationTokenSource? _cancellationTokenSource;
    
    public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();
    
    public ReactiveCommand<Unit, AlbumViewModel> BuyMusicCommand { get; }

    public AlbumViewModel? SelectedAlbum
    {
        get => _selectedAlbum;
        set
        {
            Console.WriteLine($"Selected album changed to: {value}");
            this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
        }
    }

    public string? SearchText
    {
        get => _searchText;
        set
        {
            Console.WriteLine($"Search text changed to: {value}");
            this.RaiseAndSetIfChanged(ref _searchText, value);   
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            Console.WriteLine($"IsBusy changed to: {value}");
            this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
    }

    public MusicStoreViewModel()
    {
        BuyMusicCommand = ReactiveCommand.Create(() =>
        {
            Console.WriteLine("In BuyMusicCommand");
            return SelectedAlbum;
        });

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(DoSearch!);
    }

    private async void DoSearch(string s)
    {
        SearchResults.Clear();
        
        // TODO: Understand how this cancels the load method.
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        if (!string.IsNullOrWhiteSpace(s))
        {
            IsBusy = true;
            
            var albums = await Album.SearchAsync(s);

            foreach (var album in albums)
            {
                var vm = new AlbumViewModel(album);

                SearchResults.Add(vm);
            }
            
            if (!cancellationToken.IsCancellationRequested)
            {
                LoadCovers(cancellationToken);
            }

            IsBusy = false;
        }
    }
    
    private async void LoadCovers(CancellationToken cancellationToken)
    {
        foreach (var album in SearchResults.ToList())
        {
            await album.LoadCover();

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }
}