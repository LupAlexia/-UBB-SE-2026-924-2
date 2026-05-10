using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Fixtures;

public static class UserFixture
{
    private const int DefaultTestUserId = 1;

    public static Customer CreateValidTestUser(
        string email = "andrei.ionescu@gmail.com",
        string username = "andrei_ionescu",
        string phone = "0722112233",
        Membership? membership = null)
    {
        return new Customer
        {
            Id = DefaultTestUserId,
            Email = email,
            Username = username,
            Phone = phone,
            Membership = membership,
        };
    }

    public static Customer CreateBasicTestUser()
    {
        return CreateValidTestUser(
            email: "elena.dumitru@yahoo.ro",
            username: "elena_d88",
            phone: "0744556677");
    }
}
