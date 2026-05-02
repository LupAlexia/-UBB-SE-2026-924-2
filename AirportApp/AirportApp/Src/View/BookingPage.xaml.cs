using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using AirportApp;
using AirportApp.Src.Domain;
using AirportApp.Src.ViewModel;

namespace AirportApp.Src.View
{
    public sealed partial class BookingPage : Page
    {
        private const byte ColorAlphaFull = 255;
        private const byte OccupiedSeatColorR = 156;
        private const byte OccupiedSeatColorG = 163;
        private const byte OccupiedSeatColorB = 175;
        private const byte SelectedSeatColorR = 43;
        private const byte SelectedSeatColorG = 184;
        private const byte SelectedSeatColorB = 192;
        private const byte AvailableSeatColorR = 229;
        private const byte AvailableSeatColorG = 231;
        private const byte AvailableSeatColorB = 235;
        private const int SeatColumnsCount = 6;
        private const int SeatColumnWidth = 58;
        private const int AisleColumnIndex = 3;
        private const int AisleColumnWidth = 24;
        private const int SeatRowHeight = 54;
        private const int SeatButtonWidth = 50;
        private const int SeatButtonHeight = 44;
        private const int SeatButtonFontSize = 13;
        private const int SeatButtonMargin = 2;

        public BookingViewModel ViewModel { get; }
        private PassengerFormViewModel? seatTargetPassenger;
        private readonly SolidColorBrush occupiedSeatBrush = new SolidColorBrush(ColorHelper.FromArgb(ColorAlphaFull, OccupiedSeatColorR, OccupiedSeatColorG, OccupiedSeatColorB));
        private readonly SolidColorBrush selectedSeatBrush = new SolidColorBrush(ColorHelper.FromArgb(ColorAlphaFull, SelectedSeatColorR, SelectedSeatColorG, SelectedSeatColorB));
        private readonly SolidColorBrush availableSeatBrush = new SolidColorBrush(ColorHelper.FromArgb(ColorAlphaFull, AvailableSeatColorR, AvailableSeatColorG, AvailableSeatColorB));

        public BookingPage()
        {
            this.InitializeComponent();

            ViewModel = new BookingViewModel(App.BookingService, App.PricingService, App.NavigationService);
            ViewModel.Passengers.CollectionChanged += Passengers_CollectionChanged;
            ViewModel.BookingConfirmed += ViewModel_BookingConfirmed;

            this.DataContext = ViewModel;
        }

        private async void ViewModel_BookingConfirmed(object? sender, EventArgs eventArguments)
        {
            var dialog = new ContentDialog
            {
                Title = "Booking confirmed",
                Content = "Your tickets were successfully booked.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
            App.NavigationService.NavigateTo(typeof(FlightSearchPage));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs eventArguments)
        {
            base.OnNavigatedTo(eventArguments);

            bool initialized = await ViewModel.OnNavigatedToAsync(eventArguments.Parameter);

            if (initialized)
            {
                GenerateSeatMap();
                EnsureSeatTargetPassenger();
                RefreshSeatMapVisuals();
            }
        }

        private void GenerateSeatMap()
        {
            seatMapGrid.Children.Clear();
            seatMapGrid.RowDefinitions.Clear();
            seatMapGrid.ColumnDefinitions.Clear();

            for (int columnIndex = 0; columnIndex < SeatColumnsCount; columnIndex++)
            {
                seatMapGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(SeatColumnWidth) });
            }

            seatMapGrid.ColumnDefinitions.Insert(AisleColumnIndex, new ColumnDefinition() { Width = new GridLength(AisleColumnWidth) });

            for (int rowIndex = 0; rowIndex < ViewModel.SeatMapRowCount; rowIndex++)
            {
                seatMapGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SeatRowHeight) });
            }

            foreach (var seatDescriptor in ViewModel.SeatMapLayout)
            {
                CreateSeatButton(seatDescriptor.Row, seatDescriptor.Column, seatDescriptor.Label);
            }

            RefreshSeatMapVisuals();
        }

        private void CreateSeatButton(int rowIndex, int columnIndex, string seatNumber)
        {
            Button seatButton = new Button
            {
                Content = seatNumber,
                Width = SeatButtonWidth,
                Height = SeatButtonHeight,
                Margin = new Thickness(SeatButtonMargin),
                Padding = new Thickness(0),
                FontSize = SeatButtonFontSize,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(seatButton, rowIndex);
            Grid.SetColumn(seatButton, columnIndex);

            if (ViewModel.OccupiedSeats.Contains(seatNumber))
            {
                seatButton.IsHitTestVisible = false;
                seatButton.Background = occupiedSeatBrush;
                seatButton.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                seatButton.Background = availableSeatBrush;
                seatButton.Click += Seat_Click;
            }

            seatMapGrid.Children.Add(seatButton);
        }

        private void Seat_Click(object? sender, RoutedEventArgs eventArguments)
        {
            if (sender is Button seatButton && seatButton.Content is string selectedSeat)
            {
                EnsureSeatTargetPassenger();
                if (seatTargetPassenger == null)
                {
                    return;
                }

                ViewModel.SelectSeat(seatTargetPassenger, selectedSeat);
                RefreshSeatMapVisuals();
            }
        }

        private void Passengers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArguments)
        {
            EnsureSeatTargetPassenger();
            RefreshSeatMapVisuals();
        }

        private void SeatPassengerSelector_SelectionChanged(object? sender, SelectionChangedEventArgs eventArguments)
        {
            seatTargetPassenger = seatPassengerSelector.SelectedItem as PassengerFormViewModel;
            RefreshSeatMapVisuals();
        }

        private void EnsureSeatTargetPassenger()
        {
            if (seatTargetPassenger != null && ViewModel.Passengers.Contains(seatTargetPassenger))
            {
                return;
            }

            seatTargetPassenger = ViewModel.Passengers.FirstOrDefault();
            seatPassengerSelector.SelectedItem = seatTargetPassenger;
        }

        private void RefreshSeatMapVisuals()
        {
            foreach (var seatButton in seatMapGrid.Children.OfType<Button>())
            {
                if (seatButton.Content is not string seatNumber)
                {
                    continue;
                }

                if (ViewModel.OccupiedSeats.Contains(seatNumber))
                {
                    seatButton.IsEnabled = true;
                    seatButton.IsHitTestVisible = false;
                    seatButton.Background = occupiedSeatBrush;
                    seatButton.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    seatButton.IsEnabled = true;
                    seatButton.IsHitTestVisible = true;
                    bool isSelectedByPassenger = ViewModel.Passengers.Any(passenger => passenger.SelectedSeat == seatNumber);
                    seatButton.Background = isSelectedByPassenger ? selectedSeatBrush : availableSeatBrush;
                    seatButton.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void AddOnList_SelectionChanged(object? sender, SelectionChangedEventArgs eventArguments)
        {
            if (sender is ListView listView && listView.Tag is PassengerFormViewModel passenger)
            {
                ViewModel.UpdatePassengerAddOns(
                    passenger,
                    System.Linq.Enumerable.OfType<AddOn>(eventArguments.AddedItems),
                    System.Linq.Enumerable.OfType<AddOn>(eventArguments.RemovedItems));
            }
        }
    }
}