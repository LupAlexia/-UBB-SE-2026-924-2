using System;
using AirportApp.Src.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.General
{
    public sealed partial class LandingPage : Page
    {
        private DispatcherTimer carouselTimer = new DispatcherTimer();

        public LandingViewModel ViewModel { get; }

        public LandingPage()
        {
            ViewModel = (App.Current as App).Services.GetService<LandingViewModel>();
            this.InitializeComponent();
            this.DataContext = ViewModel;
            StartCarousel();
            this.Loaded += (sender, arguments) => ViewModel.LoadReviews();
        }

        private void StartCarousel()
        {
            carouselTimer.Interval = TimeSpan.FromSeconds(2);
            carouselTimer.Tick += OnCarouselTick;
            carouselTimer.Start();
        }

        private void OnCarouselTick(object? sender, object arguments)
        {
            if (SupportCarousel.Items.Count <= 1)
            {
                return;
            }

            int nextIndex = (SupportCarousel.SelectedIndex + 1) % SupportCarousel.Items.Count;
            SupportCarousel.SelectedIndex = nextIndex;
        }
    }
}