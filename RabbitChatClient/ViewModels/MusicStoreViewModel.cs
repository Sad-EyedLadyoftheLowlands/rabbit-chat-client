using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class MusicStoreViewModel : ViewModelBase
{
    private string? _searchText;
    private bool _isBusy;
    private AlbumViewModel? _selectedAlbum; // Why is this a ViewModel and not just a model? Because it is displayed. 

    public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

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
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    public MusicStoreViewModel()
    {
        SearchResults.Add(new AlbumViewModel());
        SearchResults.Add(new AlbumViewModel());
        SearchResults.Add(new AlbumViewModel());
    }
}