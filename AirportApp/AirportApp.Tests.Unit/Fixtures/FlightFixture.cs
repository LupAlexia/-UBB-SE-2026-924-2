using TicketManager.Domain;

namespace TicketManager.Tests.Unit.Fixtures;

public static class FlightFixture
{
    public static Flight CreateValidTestFlight(
        int flightId = 1,
        string flightNumber = "RO101",
        DateTime? departureTime = null,
        DateTime? arrivalTime = null,
        int capacity = 180)
    {
        var departure = departureTime ?? DateTime.Now.AddDays(3);
        var arrival = arrivalTime ?? departure.AddMinutes(80);

        var route = new Route
        {
            RouteId = 1,
            Company = new Company { CompanyId = 1, Name = "Tarom" },
            Airport = new Airport { AirportId = 1, City = "Otopeni" },
            RouteType = "OneWay",
            DepartureTime = departure,
            ArrivalTime = arrival,
            Capacity = capacity
        };

        return new Flight
        {
            FlightId = flightId,
            FlightNumber = flightNumber,
            Route = route,
            Gate = new Gate { GateId = 1, GateName = "A1" },
            Date = departure
        };
    }

    public static Flight CreateFlightWithDurationMinutes(int durationMinutes)
    {
        var departure = DateTime.Now.AddDays(5);
        var arrival = departure.AddMinutes(durationMinutes);

        var route = new Route
        {
            RouteId = 1,
            Company = new Company { CompanyId = 1, Name = "Tarom" },
            Airport = new Airport { AirportId = 1, City = "Otopeni" },
            RouteType = "OneWay",
            DepartureTime = departure,
            ArrivalTime = arrival,
            Capacity = 180
        };

        return new Flight
        {
            FlightId = 1,
            FlightNumber = "RO101",
            Route = route,
            Gate = new Gate { GateId = 1, GateName = "A1" },
            Date = departure
        };
    }

    public static Flight CreateFlightWithoutRoute()
    {
        return new Flight
        {
            FlightId = 1,
            FlightNumber = "RO999",
            Route = null,
            Gate = new Gate { GateId = 1, GateName = "A1" },
            Date = DateTime.Now.AddDays(1)
        };
    }

    public static Flight CreateFlightWithShortDuration()
    {
        return CreateFlightWithDurationMinutes(10);
    }

    public static Flight CreateFlightWithLongDuration()
    {
        return CreateFlightWithDurationMinutes(480);
    }
}

