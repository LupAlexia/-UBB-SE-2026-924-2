using System;
using AirportApp.Src.View.Faq;
using AirportApp.Src.View.General;
using AirportApp.Src.View.Ticket;
using AirportApp.Src.ViewModel.Review;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Review
{
    public sealed partial class ReviewPage : Page
    {
        public AddReviewViewModel ViewModel { get; }

        public ReviewPage()
        {
            this.InitializeComponent();
            ViewModel = (App.Current as App).Services.GetService<AddReviewViewModel>();

            this.DataContext = ViewModel;
            ViewModel.AlertRequested += OnAlertRequested;

            this.Unloaded += (sender, eventArguments) =>
            {
                ViewModel.AlertRequested -= OnAlertRequested;
            };
        }

        private async void OnAlertRequested(object? sender, (string Title, string Message) args)
        {
            var dialog = new MaiBoule(args.Message, args.Title)
            {
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private async void NavigateToTicketsView_Click(object sender, RoutedEventArgs arguments)
        {
            var button = sender as Button;

            Frame.Navigate(typeof(TicketsView));
        }
    }
}

