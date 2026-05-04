using System;
using AirportApp.Src.ViewModel;
using AirportApp.Src.ViewModel.Chats;
using AirportApp.Src.ViewModel.General;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Chat
{
    public sealed partial class ChatPage : Page
    {
        public ChatViewModel ViewModel { get; }
        public ChatPage()
        {
            ViewModel = (App.Current as App).Services.GetService<ChatViewModel>();
            this.InitializeComponent();
        }

        public async void EndChat(object sender, RoutedEventArgs arguments)
        {
            await ViewModel.CloseChatAsync();
            this.Frame.Navigate(typeof(AirportApp.Src.View.General.LandingPage));
        }
    }
}