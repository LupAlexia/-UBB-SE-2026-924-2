using AirportApp.ClassLibrary.Entity.Domain;
using FluentAssertions;
using Moq;
using AirportApp.Src.Service;
using AirportApp.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
        var FlightTicket = new FlightTicket { Id = ActiveId, Status = ActiveStatus };
        mockCancellationService.Setup(serviceAllowingCancel => serviceAllowingCancel.CanCancelTicket(FlightTicket)).Returns((true, string.Empty));

        viewModel.CancelTicketCommand.Execute(FlightTicket);

        viewModel.PendingCancelTicket.Should().Be(FlightTicket);
    }

    [TestMethod]
    public void CancelTicketCommand_NotCancelableTicket_SetsCancellationFailed()
    {
        var FlightTicket = new FlightTicket { Id = ActiveId, Status = ActiveStatus };
        mockCancellationService.Setup(serviceDenyingCancel => serviceDenyingCancel.CanCancelTicket(FlightTicket))
            .Returns((false, CancellationReasonMessage));

        viewModel.CancelTicketCommand.Execute(FlightTicket);

        viewModel.CancellationSucceeded.Should().BeFalse();
        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [TestMethod]
    public void CancelTicketCommand_CancelledTicket_Ignores()
    {
        var FlightTicket = new FlightTicket { Id = CancelledId, Status = CancelledStatus };

        viewModel.CancelTicketCommand.Execute(FlightTicket);

        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [TestMethod]
    public async Task ConfirmCancellation_Invoked_CallsServiceAndClearsState()
    {
        UserSession.CurrentUser = new Customer { Id = TestUserId, Email = TestEmail };
        var FlightTicket = new FlightTicket { Id = TargetTicketIdToCancel, Status = ActiveStatus };
        viewModel.PendingCancelTicket = FlightTicket;
        mockDashboardService.Setup(dashboardServiceReturningNoTickets => dashboardServiceReturningNoTickets.GetUserTicketsAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new List<FlightTicket>());

        await viewModel.ConfirmCancellationAsync();

        mockCancellationService.Verify(cancellationServiceToVerifyCancel => cancellationServiceToVerifyCancel.CancelTicketAsync(TargetTicketIdToCancel), Times.Once);
        viewModel.PendingCancelTicket.Should().BeNull();
        viewModel.CancellationSucceeded.Should().BeTrue();
    }

    [TestMethod]
    public void DeclineCancellation_Invoked_ClearsPendingTicket()
    {
        var FlightTicket = new FlightTicket { Id = PendingId, Status = ActiveStatus };
        viewModel.PendingCancelTicket = FlightTicket;

        viewModel.DeclineCancellation();

        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [TestMethod]
    public async Task OnNavigatedTo_NotAuthenticated_RedirectsToAuthentication()
    {
        UserSession.CurrentUser = null;

        var navigationResult = await viewModel.OnNavigatedToAsync();

        navigationResult.Should().BeFalse();
        mockNavigationService.Verify(navServiceToVerifyAuthRedirect => navServiceToVerifyAuthRedirect.NavigateTo(It.IsAny<Type>(), It.IsAny<object?>()), Times.AtLeastOnce);
    }
}
