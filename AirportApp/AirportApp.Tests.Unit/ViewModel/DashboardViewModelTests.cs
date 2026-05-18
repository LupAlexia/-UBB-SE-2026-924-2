using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AirportApp.ClassLibrary.Entity.Domain;
using FluentAssertions;
using Moq;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Tests.Unit.ViewModel;

[TestClass]
[DoNotParallelize]
public class DashboardViewModelTests
{
    private const int ActiveId = 1;
    private const int CancelledId = 2;
    private const int TargetTicketIdToCancel = 5;
    private const int PendingId = 3;
    private const int TestUserId = 1;
    private const string TestEmail = "bogdan.ionescu@gmail.com";
    private const string UpcomingFilter = "Upcoming";
    private const string ActiveStatus = "Active";
    private const string CancelledStatus = "Cancelled";
    private const string CancellationReasonMessage = "Cannot cancel within 24 hours of departure";

    private readonly Mock<IDashboardService> mockDashboardService;
    private readonly Mock<ICancellationService> mockCancellationService;
    private readonly Mock<INavigationService> mockNavigationService;
    private readonly DashboardViewModel viewModel;

    public DashboardViewModelTests()
    {
        mockDashboardService = new Mock<IDashboardService>();
        mockCancellationService = new Mock<ICancellationService>();
        mockNavigationService = new Mock<INavigationService>();
        viewModel = new DashboardViewModel(mockDashboardService.Object, mockCancellationService.Object, mockNavigationService.Object);
    }

    [TestMethod]
    public void CancelTicketCommand_CancelableTicket_SetsPending()
    {
        var flightTicket = new FlightTicket { Id = ActiveId, Status = ActiveStatus };
        mockCancellationService.Setup(serviceAllowingCancel => serviceAllowingCancel.CanCancelTicket(flightTicket)).Returns((true, string.Empty));

        viewModel.CancelTicketCommand.Execute(flightTicket);

        viewModel.PendingCancelTicket.Should().Be(flightTicket);
    }

    [TestMethod]
    public void CancelTicketCommand_NotCancelableTicket_SetsCancellationFailed()
    {
        var flightTicket = new FlightTicket { Id = ActiveId, Status = ActiveStatus };
        mockCancellationService.Setup(serviceDenyingCancel => serviceDenyingCancel.CanCancelTicket(flightTicket))
            .Returns((false, CancellationReasonMessage));

        viewModel.CancelTicketCommand.Execute(flightTicket);

        viewModel.CancellationSucceeded.Should().BeFalse();
        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [TestMethod]
    public void CancelTicketCommand_CancelledTicket_Ignores()
    {
        var flightTicket = new FlightTicket { Id = CancelledId, Status = CancelledStatus };

        viewModel.CancelTicketCommand.Execute(flightTicket);

        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [TestMethod]
    public async Task ConfirmCancellation_Invoked_CallsServiceAndClearsStateAsync()
    {
        UserSession.CurrentUser = new Customer { Id = TestUserId, Email = TestEmail };
        var flightTicket = new FlightTicket { Id = TargetTicketIdToCancel, Status = ActiveStatus };
        viewModel.PendingCancelTicket = flightTicket;
        mockDashboardService.Setup(dashboardServiceReturningNoTickets => dashboardServiceReturningNoTickets.GetUserTicketsAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new List<FlightTicket>());

        await viewModel.ConfirmCancellationAsync();

        mockCancellationService.Verify(cancellationServiceToVerifyCancel => cancellationServiceToVerifyCancel.CancelTicketAsync(TargetTicketIdToCancel), Times.Once);
        viewModel.PendingCancelTicket.Should().BeNull();
        viewModel.CancellationSucceeded.Should().BeTrue();
    }

    [TestMethod]
    public void DeclineCancellation_Invoked_ClearsPendingTicket()
    {
        var flightTicket = new FlightTicket { Id = PendingId, Status = ActiveStatus };
        viewModel.PendingCancelTicket = flightTicket;

        viewModel.DeclineCancellation();

        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [TestMethod]
    public async Task OnNavigatedToAsync_NotAuthenticated_RedirectsToAuthenticationAsync()
    {
        UserSession.CurrentUser = null;

        var navigationResult = await viewModel.OnNavigatedToAsync();

        navigationResult.Should().BeFalse();
        mockNavigationService.Verify(navServiceToVerifyAuthRedirect => navServiceToVerifyAuthRedirect.NavigateTo(It.IsAny<Type>(), It.IsAny<object?>()), Times.AtLeastOnce);
    }
}
