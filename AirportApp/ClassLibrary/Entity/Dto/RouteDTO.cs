using System;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record RouteDTO(int id, string routeType, DateTime departureTime, DateTime arrivalTime, int capacity, AirportDTO? airport = null, CompanyDTO? company = null);
}
