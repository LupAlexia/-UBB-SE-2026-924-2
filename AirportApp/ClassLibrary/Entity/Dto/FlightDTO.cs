using System;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record FlightDTO(int id, int routeId, int gateId, DateTime date, string flightNumber, RouteDTO? route);
}
