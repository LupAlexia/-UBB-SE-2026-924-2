using AirportApp.ClassLibrary.Entity.Domain.Ticket;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface ITicketRepository : IRepository<int, Ticket>
    {

        Task UpdateStatusByIdAsync(int id, TicketStatusEnum newStatus);

        Task UpdateUrgencyLevelByIdAsync(int id, TicketUrgencyLevelEnum newUrgencyLevel);
    }
}