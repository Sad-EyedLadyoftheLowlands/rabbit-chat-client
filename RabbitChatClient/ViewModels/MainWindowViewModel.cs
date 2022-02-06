using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using ReactiveUI;

namespace RabbitChatClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        public string Test => "My name is dindu nuffin";
        public List<int> MyList { get; set; }
        public ICommand BuyMusicCommand { get; }
        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

        public MainWindowViewModel()
        {
            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();
            
            BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MusicStoreViewModel();
                var result = await ShowDialog.Handle(store);
                
                Console.WriteLine("In reactive command");
            });

            MyList = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                MyList.Add(i);
            }
        }
    }
}