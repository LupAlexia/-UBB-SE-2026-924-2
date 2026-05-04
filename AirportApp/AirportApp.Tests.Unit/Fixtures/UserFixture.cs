using TicketManager.Domain;

namespace TicketManager.Tests.Unit.Fixtures;

public static class UserFixture
{
    private const int DefaultTestUserId = 1;

    public static User CreateValidTestUser(
        string email = "andrei.ionescu@gmail.com",
        string username = "andrei_ionescu",
        string phone = "0722112233",
        Membership? membership = null)
    {
        return new User
        {
            UserId = DefaultTestUserId,
            Email = email,
            Username = username,
            Phone = phone,
            Membership = membership
        };
    }

    public static User CreateBasicTestUser()
    {
        return CreateValidTestUser(
            email: "elena.dumitru@yahoo.ro",
            username: "elena_d88",
            phone: "0744556677");
    }
}
