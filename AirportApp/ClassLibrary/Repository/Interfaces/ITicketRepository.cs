using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ITicketRepository : IRepository<int, ComplaintTicket>
    {

        Task UpdateStatusByIdAsync(int id, ComplaintTicketStatusEnum newStatus);

        Task UpdateUrgencyLevelByIdAsync(int id, ComplaintTicketUrgencyLevelEnum newUrgencyLevel);
    }
}