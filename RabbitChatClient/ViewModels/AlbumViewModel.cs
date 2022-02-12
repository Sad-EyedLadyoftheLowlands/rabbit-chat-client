using System;
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
    
    public async Task SaveToDiskAsync()
    {
        await _album.SaveAsync();

        if (Cover != null)
        {
            var bitmap = Cover;

            await Task.Run(() =>
            {
                using (var fs = _album.SaveCoverBitmapSteam())
                {
                    bitmap.Save(fs);
                }
            });
        }
    }

    public void Test()
    {
        Console.WriteLine("In test");
    }
}