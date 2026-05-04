using System.ComponentModel;
using Xunit;
using TicketManager.ViewModel;

namespace TicketManager.Tests.Unit.ViewModel
{
    public class PassengerFormViewModelTests
    {
        [Fact]
        public void Initialization_DefaultState_SetsEmptyDefaults()
        {
            var viewModel = new PassengerFormViewModel();

            Assert.Equal(string.Empty, viewModel.FirstName);
            Assert.Equal(string.Empty, viewModel.LastName);
            Assert.Equal(string.Empty, viewModel.Email);
            Assert.Equal(string.Empty, viewModel.Phone);
            Assert.Equal(string.Empty, viewModel.SelectedSeat);
            Assert.NotNull(viewModel.SelectedAddOns);
            Assert.Empty(viewModel.SelectedAddOns);
        }

        [Fact]
        public void SetFirstName_NewValue_FiresPropertyChangedEvent()
        {
            const string ExpectedPropertyName = "FirstName";
            const string NewFirstNameValue = "John";

            var viewModel = new PassengerFormViewModel();
            bool eventFired = false;

            viewModel.PropertyChanged += (object? sender, PropertyChangedEventArgs eventArguments) =>
            {
                if (eventArguments.PropertyName == ExpectedPropertyName)
                {
                    eventFired = true;
                }
            };

            viewModel.FirstName = NewFirstNameValue;

            Assert.True(eventFired, "PropertyChanged event should be fired when FirstName is updated.");
            Assert.Equal(NewFirstNameValue, viewModel.FirstName);
        }
    }
}
