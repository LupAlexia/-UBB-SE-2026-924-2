using System;
using AirportApp.Src.Domain;
using AirportApp.Src.Repository;

namespace AirportApp.Src.Service
{
    public class CancellationService : ICancellationService
    {
        private readonly IFlightTicketRepository ticketRepository;

        public CancellationService(IFlightTicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        }

        public (bool CanCancel, string Reason) CanCancelTicket(FlightTicket ticket)
        {
            if (ticket == null)
            {
                return (false, "Ticket not found.");
            }

            if (string.Equals(ticket.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "This ticket is already cancelled.");
            }

            if (ticket.Flight != null && ticket.Flight.Date < DateTime.Now)
            {
                return (false, "This flight is already in the past and cannot be cancelled.");
            }

            return (true, string.Empty);
        }

        public void CancelTicket(int ticketId)
        {
            ticketRepository.UpdateTicketStatus(ticketId, "Cancelled");
        }
    }
}
