using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RabbitChatClient.Views;

public partial class FriendView : UserControl
{
    public FriendView()
    {
        InitializeComponent();
        
        // this.WhenAct
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}