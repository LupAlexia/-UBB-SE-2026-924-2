using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain.Faq;
using AirportApp.ClassLibrary.Repository;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace AirportApp.Src.View.Faq
{
    public sealed partial class FAQView : Page
    {
        public FAQViewModel ViewModel { get; }

        private int currentPersonId;

        public FAQView()
        {
            ViewModel = (App.Current as App).Services.GetService<FAQViewModel>();
            this.InitializeComponent();
            this.DataContext = ViewModel;

            UpdateAdminVisibility();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs arguments)
        {
            base.OnNavigatedTo(arguments);

            var app = (App)App.Current;

            ViewModel.IsAdmin = app.IsEmployee;

            if (app.IsEmployee && app.Employee != null)
            {
                currentPersonId = app.Employee.RetrieveUniqueDatabaseIdentifierForBot();
            }
            else if (app.User != null)
            {
                currentPersonId = app.User.RetrieveUniqueDatabaseIdentifierForBot();
            }

            await ViewModel.LoadFAQAsync();
            UpdateAdminVisibility();
        }

        private void OpenFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (sender is Button button && button.DataContext is FAQEntryDTO faq)
            {
                ViewModel.ToggleFAQ(faq);
                ScrollToMiddleIfExpanded(faq);
            }
        }

        private void AccordionHeader_Click(object sender, RoutedEventArgs arguments)
        {
            if (sender is Button button && button.DataContext is FAQEntryDTO faq)
            {
                ViewModel.ToggleFAQ(faq);
                ScrollToMiddleIfExpanded(faq);
            }
        }

        private void AllQuestionsButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel.SetCategory(FAQCategoryEnum.All);
            SetCategoryUI(AllQuestionsButton);
        }

        private void CheckInButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel.SetCategory(FAQCategoryEnum.CheckIn);
            SetCategoryUI(CheckInButton);
        }

        private void ParkingButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Parking);
            SetCategoryUI(ParkingButton);
        }

        private void BaggageButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Baggage);
            SetCategoryUI(BaggageButton);
        }

        private void TicketButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Tickets);
            SetCategoryUI(TicketsButton);
        }

        private void FacilitiesButton_Click(object sender, RoutedEventArgs arguments)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Facilities);
            SetCategoryUI(FacilitiesButton);
        }

        private void AddFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            var data = new FAQNavigationData
            {
                CurrentPersonId = currentPersonId,
                IsEmployee = ViewModel.IsAdmin,
                FAQEntry = null
            };

            Frame.Navigate(typeof(FAQAddEditPage), data);
        }

        private void EditFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (ViewModel.SelectedFAQEntry == null)
            {
                return;
            }

            var data = ViewModel.BuildNavigationData(currentPersonId);

            Frame.Navigate(typeof(FAQAddEditPage), data);
        }

        private async void DeleteFaqButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (ViewModel.SelectedFAQEntry == null)
            {
                return;
            }

            var faq = ViewModel.SelectedFAQEntry;

            var dialog = new ContentDialog
            {
                Title = "Delete FAQ",
                Content = $"Are you sure you want to delete \"{faq.Question}\"?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.DeleteFAQEntryAsync(faq);
                ViewModel.SelectedFAQEntry = null;
            }
        }

        private async void HelpfulButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (sender is Button button && button.Tag is FAQEntryDTO faq)
            {
                await ViewModel.GiveFeedbackAsync(faq, true);
            }
        }

        private async void NotHelpfulButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (sender is Button button && button.Tag is FAQEntryDTO faq)
            {
                await ViewModel.GiveFeedbackAsync(faq, false);
            }
        }

        private void UpdateAdminVisibility()
        {
            EmployeeActionsPanel.Visibility = ViewModel.IsAdmin
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void ScrollToMiddleIfExpanded(FAQEntryDTO faq)
        {
            if (faq.IsExpanded)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    var container = AllQuestionsList.ContainerFromItem(faq) as FrameworkElement;
                    if (container != null)
                    {
                        container.StartBringIntoView(new BringIntoViewOptions
                        {
                            VerticalAlignmentRatio = 0.5,
                            AnimationDesired = true
                        });
                    }
                });
            }
        }

        private void SetCategoryUI(Button selected)
        {
            var normal = (Style)this.Resources["CategoryButtonStyle"];
            var active = (Style)this.Resources["SelectedCategoryButtonStyle"];

            AllQuestionsButton.Style = normal;
            CheckInButton.Style = normal;
            ParkingButton.Style = normal;
            BaggageButton.Style = normal;
            TicketsButton.Style = normal;
            FacilitiesButton.Style = normal;

            selected.Style = active;
        }
    }
}