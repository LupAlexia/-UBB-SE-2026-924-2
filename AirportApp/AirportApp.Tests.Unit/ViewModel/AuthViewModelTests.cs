using System;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AirportApp.ClassLibrary.Entity.Domain;
using FluentAssertions;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Tests.Unit.ViewModel
{
    [TestClass]
    public class AuthViewModelTests
    {
        private IAuthService mockAuthService;
        private INavigationService mockNavigationService;
        private AuthViewModel authViewModel;

        [TestInitialize]
        public void Setup()
        {
            mockAuthService = Substitute.For<IAuthService>();
            mockNavigationService = Substitute.For<INavigationService>();
            authViewModel = new AuthViewModel(mockAuthService, mockNavigationService);
            UserSession.PendingBookingParameters = null; // Add this line
        }

        [TestMethod]
        public void ToggleMode_WhenCalled_TogglesIsLoginMode()
        {
            // Arrange
            authViewModel.IsLoginMode = true;

            // Act
            authViewModel.ToggleModeCommand.Execute(null);

            // Assert
            authViewModel.IsLoginMode.Should().BeFalse();

            // Act again
            authViewModel.ToggleModeCommand.Execute(null);

            // Assert again
            authViewModel.IsLoginMode.Should().BeTrue();
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_NavigatesToSearchAsync()
        {
            // Arrange
            var customer = new Customer { Email = "test@test.com", Username = "test" };
            authViewModel.EmailText = "test@test.com";
            authViewModel.PasswordText = "password";
            authViewModel.IsLoginMode = true;
            mockAuthService.LoginAsync(authViewModel.EmailText, authViewModel.PasswordText).Returns(customer);

            // Act
            authViewModel.ActionCommand.Execute(null);
            await Task.Delay(100); // Wait for async command

            // Assert
            authViewModel.IsAuthenticated.Should().BeTrue();
            mockNavigationService.Received(1).NavigateTo(Arg.Is<Type>(t => t.Name == "FlightSearchPage"), Arg.Any<object>());
        }

        [TestMethod]
        public async Task RegisterAsync_ValidData_SetsSuccessMessageAsync()
        {
            // Arrange
            authViewModel.IsLoginMode = false;
            authViewModel.EmailText = "new@test.com";
            authViewModel.UsernameText = "newuser";
            authViewModel.PhoneText = "0712345678";
            authViewModel.PasswordText = "password123";

            // Act
            authViewModel.ActionCommand.Execute(null);
            await Task.Delay(100);

            // Assert
            await mockAuthService.Received(1).RegisterAsync(
                authViewModel.EmailText,
                authViewModel.PhoneText,
                authViewModel.UsernameText,
                authViewModel.PasswordText);
            authViewModel.SuccessMessage.Should().Contain("Registration successful");
        }

        [TestMethod]
        public void IsFormValid_LoginMode_ReturnsTrueOnlyWhenEmailAndPasswordSet()
        {
            // Arrange
            authViewModel.IsLoginMode = true;
            authViewModel.EmailText = string.Empty;
            authViewModel.PasswordText = string.Empty;

            // Assert
            authViewModel.IsFormValid.Should().BeFalse();

            // Act
            authViewModel.EmailText = "test@test.com";
            authViewModel.PasswordText = "pass";

            // Assert
            authViewModel.IsFormValid.Should().BeTrue();
        }
    }
}
