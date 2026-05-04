using TicketManager.Domain;

namespace TicketManager.Tests.Unit.Fixtures;

public static class PassengerDataFixture
{
    public static PassengerData CreateValidPassengerData(
        string firstName = "Alexandru",
        string lastName = "Miron",
        string email = "alex.miron@gmail.com",
        string phone = "0733112233",
        string selectedSeat = "1A")
    {
        return new PassengerData
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            SelectedSeat = selectedSeat,
            SelectedAddOns = new List<AddOn>()
        };
    }

    public static List<PassengerData> CreateValidPassengerList(int count = 2)
    {
        var randomPrefix = Guid.NewGuid().ToString().Substring(0, 4);
        var passengers = new List<PassengerData>();
        var firstNames = new[] { "Mihai", "Simona", "Bogdan", "Raluca", "Cristian", "Adina", "Florin" };
        var lastNames = new[] { "Popa", "Stan", "Diaconu", "Ungureanu", "Vasile", "Lupu", "Nistor" };

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[i % firstNames.Length];
            var lastName = lastNames[i % lastNames.Length];
            passengers.Add(CreateValidPassengerData(
                firstName: firstName,
                lastName: lastName,
                email: $"{firstName.ToLower()}.{lastName.ToLower()}_{randomPrefix}@yahoo.com",
                selectedSeat: $"{randomPrefix}_{i + 1}B"));
        }
        return passengers;
    }
}


