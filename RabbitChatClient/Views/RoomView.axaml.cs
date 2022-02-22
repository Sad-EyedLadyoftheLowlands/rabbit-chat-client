using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using RabbitChatClient.ViewModels;

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

    private void MyTextInput_OnKeyDown(object? sender, KeyEventArgs e)
    {
        var x = (RoomViewModel)this.DataContext;
        Console.WriteLine("huh?");
        
        // throw new System.NotImplementedException();
    }
}