using FluentAssertions;
using Moq;
using TicketManager.Domain;
using TicketManager.Service;
using TicketManager.ViewModel;

namespace TicketManager.Tests.Unit.ViewModel;

public class DashboardViewModelTests
{
    private const int ActiveTicketId = 1;
    private const int CancelledTicketId = 1;
    private const int TargetTicketIdToCancel = 5;
    private const int PendingTicketId = 3;
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

    [Fact]
    public void CancelTicketCommand_CancelableTicket_SetsPending()
    {
        var ticket = new Ticket { TicketId = ActiveTicketId, Status = ActiveStatus };
        mockCancellationService.Setup(serviceAllowingCancel => serviceAllowingCancel.CanCancelTicket(ticket)).Returns((true, string.Empty));

        viewModel.CancelTicketCommand.Execute(ticket);

        viewModel.PendingCancelTicket.Should().Be(ticket);
    }

    [Fact]
    public void CancelTicketCommand_NotCancelableTicket_SetsCancellationFailed()
    {
        var ticket = new Ticket { TicketId = ActiveTicketId, Status = ActiveStatus };
        mockCancellationService.Setup(serviceDenyingCancel => serviceDenyingCancel.CanCancelTicket(ticket))
            .Returns((false, CancellationReasonMessage));

        viewModel.CancelTicketCommand.Execute(ticket);

        viewModel.CancellationSucceeded.Should().BeFalse();
        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [Fact]
    public void CancelTicketCommand_CancelledTicket_Ignores()
    {
        var ticket = new Ticket { TicketId = CancelledTicketId, Status = CancelledStatus };

        viewModel.CancelTicketCommand.Execute(ticket);

        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [Fact]
    public void ConfirmCancellation_Invoked_CallsServiceAndClearsState()
    {
        UserSession.CurrentUser = new User { UserId = TestUserId, Email = TestEmail };
        var ticket = new Ticket { TicketId = TargetTicketIdToCancel, Status = ActiveStatus };
        viewModel.PendingCancelTicket = ticket;
        mockDashboardService.Setup(dashboardServiceReturningNoTickets => dashboardServiceReturningNoTickets.GetUserTickets(It.IsAny<int>(), It.IsAny<string>())).Returns(new List<Ticket>());

        viewModel.ConfirmCancellation();

        mockCancellationService.Verify(cancellationServiceToVerifyCancel => cancellationServiceToVerifyCancel.CancelTicket(TargetTicketIdToCancel), Times.Once);
        viewModel.PendingCancelTicket.Should().BeNull();
        viewModel.CancellationSucceeded.Should().BeTrue();
    }

    [Fact]
    public void DeclineCancellation_Invoked_ClearsPendingTicket()
    {
        var ticket = new Ticket { TicketId = PendingTicketId, Status = ActiveStatus };
        viewModel.PendingCancelTicket = ticket;

        viewModel.DeclineCancellation();

        viewModel.PendingCancelTicket.Should().BeNull();
    }

    [Fact]
    public void OnNavigatedTo_NotAuthenticated_RedirectsToAuthentication()
    {
        UserSession.CurrentUser = null;

        var navigationResult = viewModel.OnNavigatedTo();

        navigationResult.Should().BeFalse();
        mockNavigationService.Verify(navServiceToVerifyAuthRedirect => navServiceToVerifyAuthRedirect.NavigateTo(typeof(View.AuthPage), null), Times.Once);
    }
}

