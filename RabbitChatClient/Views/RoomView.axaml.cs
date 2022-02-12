using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RabbitChatClient.Views;

public partial class RoomView : UserControl
{
    public RoomView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}