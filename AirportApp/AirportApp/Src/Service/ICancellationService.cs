using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public interface ICancellationService
    {
        (bool CanCancel, string Reason) CanCancelTicket(FlightTicket ticket);
        void CancelTicket(int ticketId);
    }
}
