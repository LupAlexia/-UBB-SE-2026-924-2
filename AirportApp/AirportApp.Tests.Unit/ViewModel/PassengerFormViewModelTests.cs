using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.ViewModel;

namespace AirportApp.Tests.Unit.ViewModel
{
    public class PassengerFormViewModelTests
    {
        [TestMethod]
        public void Initialization_DefaultState_SetsEmptyDefaults()
        {
            var viewModel = new PassengerFormViewModel();

            Assert.AreEqual(string.Empty, viewModel.FirstName);
            Assert.AreEqual(string.Empty, viewModel.LastName);
            Assert.AreEqual(string.Empty, viewModel.Email);
            Assert.AreEqual(string.Empty, viewModel.Phone);
            Assert.AreEqual(string.Empty, viewModel.SelectedSeat);
            Assert.IsNotNull(viewModel.SelectedAddOns);
            Assert.AreEqual(0, viewModel.SelectedAddOns.Count);
        }

        [TestMethod]
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

            Assert.IsTrue(eventFired, "PropertyChanged event should be fired when FirstName is updated.");
            Assert.AreEqual(NewFirstNameValue, viewModel.FirstName);
        }
    }
}




