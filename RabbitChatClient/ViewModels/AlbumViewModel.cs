using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using RabbitChatClient.Models;
using ReactiveUI;

namespace RabbitChatClient.ViewModels;

public class AlbumViewModel : ViewModelBase
{
    private readonly Album _album;

    private Bitmap? _cover;

    public Bitmap? Cover
    {
        get => _cover;
        private set => this.RaiseAndSetIfChanged(ref _cover, value);
    }
    
    public string Artist => _album.Artist;

    public string Title => _album.Title;

    public AlbumViewModel(Album album)
    {
        _album = album;
    }

    public async Task LoadCover()
    {
        await using (var imageStream = await _album.LoadCoverBitmapAsync())
        {
            Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
        }
    }
}