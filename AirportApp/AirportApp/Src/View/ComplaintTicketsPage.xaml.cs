using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI;
using Microsoft.Extensions.DependencyInjection;
using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Dto;
using Microsoft.UI.Xaml.Media;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.View.Ticket
{
    public sealed partial class ComplaintTicketPage : Page
    {
        private const int DEFAULT_GUEST_IDENTIFIER = 102;
        private const string DEFAULT_SYSTEM_EMAIL = "email@email.com";

        public TicketsViewModel ViewModel { get; }

        public ComplaintTicketPage()
        {
            ViewModel = (App.Current as App).Services.GetService<TicketsViewModel>();
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        private async void CreateTicketButton_Click(object sender, RoutedEventArgs arguments)
        {
            // Build the UI using the helper method to keep this handler clean
            var (layout, inputs) = BuildSubmissionForm();

            ContentDialog submissionDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Content = new ScrollViewer { MaxHeight = 500, Content = layout },
                Background = new SolidColorBrush(Colors.White),
                RequestedTheme = ElementTheme.Light
            };

            // Logic for the Send button
            inputs.SubmitButton.Click += async (sender, args) =>
            {
                await HandleSubmission(inputs, submissionDialog);
            };

            // Logic for the Cancel button
            inputs.CancelButton.Click += (sender, args) => submissionDialog.Hide();

            await submissionDialog.ShowAsync();
        }

        private async Task HandleSubmission(SubmissionFormInputs inputs, ContentDialog dialog)
        {
            try
            {
                inputs.ErrorBlock.Visibility = Visibility.Collapsed;
                if (string.IsNullOrWhiteSpace(inputs.TitleBox.Text) || string.IsNullOrWhiteSpace(inputs.DescriptionBox.Text))
                {
                    throw new Exception("Please fill all required fields.");
                }

                var selectedCategory = ViewModel.Categories.FirstOrDefault(categoryItem => categoryItem.CategoryName == inputs.CategoryCombo.SelectedItem?.ToString());
                var selectedSubcategory = ViewModel.Subcategories.FirstOrDefault(subcategoryItem => subcategoryItem.SubcategoryName == inputs.SubcategoryCombo.SelectedItem?.ToString());

                int finalCategoryId = (selectedCategory == null || selectedCategory.Id == 0) ? 1 : selectedCategory.Id;
                int finalSubcategoryId = (selectedSubcategory == null || selectedSubcategory.Id == 0) ? 1 : selectedSubcategory.Id;

                var newTicket = new TicketDTO(
                    ticketId: 0,
                    creatorAccountId: DEFAULT_GUEST_IDENTIFIER,
                    creatorEmailAddress: DEFAULT_SYSTEM_EMAIL,
                    urgencyLevel: ComplaintTicketUrgencyLevelEnum.LOW,
                    currentStatus: ComplaintTicketStatusEnum.OPEN,
                    categoryId: finalCategoryId,
                    categoryName: selectedCategory?.CategoryName ?? "General",
                    subcategoryId: finalSubcategoryId,
                    subcategoryName: selectedSubcategory?.SubcategoryName ?? "General",
                    subject: inputs.TitleBox.Text,
                    description: inputs.DescriptionBox.Text,
                    creationTimestamp: DateTime.Now);

                await ViewModel.CreateTicketAsync(newTicket);
                dialog.Hide();
            }
            catch (Exception exceptionThrown)
            {
                inputs.ErrorBlock.Text = exceptionThrown.Message;
                inputs.ErrorBlock.Visibility = Visibility.Visible;
            }
        }

        private (StackPanel Layout, SubmissionFormInputs Inputs) BuildSubmissionForm()
        {
            var panel = new StackPanel { Spacing = 12, Padding = new Thickness(10) };

            panel.Children.Add(new TextBlock { Text = "Submit a New Ticket", FontSize = 24, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });

            var titleBox = new TextBox { Header = "Title of the issue*", PlaceholderText = "e.g. Damaged suitcase", Width = 400 };
            panel.Children.Add(titleBox);

            var categoryCombo = new ComboBox { Header = "Category*", Width = 400, PlaceholderText = "Select Category" };
            foreach (var cat in ViewModel.Categories)
            {
                categoryCombo.Items.Add(cat.CategoryName);
            }
            panel.Children.Add(categoryCombo);

            var subcategoryCombo = new ComboBox { Header = "Subcategory*", Width = 400, PlaceholderText = "Select Subcategory" };
            panel.Children.Add(subcategoryCombo);

            categoryCombo.SelectionChanged += async (sender, arguments) =>
            {
                subcategoryCombo.Items.Clear();
                var cat = ViewModel.Categories.FirstOrDefault(categoryItem => categoryItem.CategoryName == categoryCombo.SelectedItem?.ToString());
                if (cat == null)
                {
                    return;
                }
                await ViewModel.LoadSubcategoriesAsync(cat.Id);
                foreach (var sub in ViewModel.Subcategories)
                {
                    subcategoryCombo.Items.Add(sub.SubcategoryName);
                }
            };

            var descriptionBox = new TextBox { Header = "Description*", PlaceholderText = "Details...", Height = 120, TextWrapping = TextWrapping.Wrap, AcceptsReturn = true };
            panel.Children.Add(descriptionBox);

            var errorBlock = new TextBlock { Foreground = new SolidColorBrush(Colors.Red), Visibility = Visibility.Collapsed, TextWrapping = TextWrapping.Wrap };
            panel.Children.Add(errorBlock);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Margin = new Thickness(0, 10, 0, 0) };
            var sendBtn = new Button { Content = "Send", Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 43, 184, 192)), Foreground = new SolidColorBrush(Colors.White) };
            var cancelBtn = new Button { Content = "Cancel" };
            btnPanel.Children.Add(sendBtn);
            btnPanel.Children.Add(cancelBtn);
            panel.Children.Add(btnPanel);

            return (panel, new SubmissionFormInputs { TitleBox = titleBox, CategoryCombo = categoryCombo, SubcategoryCombo = subcategoryCombo, DescriptionBox = descriptionBox, SubmitButton = sendBtn, CancelButton = cancelBtn, ErrorBlock = errorBlock });
        }

        private async Task ShowError(string message)
        {
            var dialog = new ContentDialog { XamlRoot = this.XamlRoot, Title = "Error", Content = message, CloseButtonText = "OK" };
            await dialog.ShowAsync();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs arguments)
        {
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem selected && selected.Tag != null)
            {
                ViewModel.SelectedFilterString = selected.Tag.ToString();
            }
        }
    }

    public class SubmissionFormInputs
    {
        public TextBox TitleBox { get; set; }
        public ComboBox CategoryCombo { get; set; }
        public ComboBox SubcategoryCombo { get; set; }
        public TextBox DescriptionBox { get; set; }
        public Button SubmitButton { get; set; }
        public Button CancelButton { get; set; }
        public TextBlock ErrorBlock { get; set; }
    }
}