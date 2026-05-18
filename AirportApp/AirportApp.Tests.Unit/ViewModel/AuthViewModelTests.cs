using System;
using System.Threading.Tasks;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AirportApp.ClassLibrary.Entity.Domain;
using FluentAssertions;
using AirportApp.ClassLibrary.Service.Interfaces;
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
