namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CustomerDTO(int id, string email, string? phone, string username, string passwordHash, int? membershipId, MembershipDTO? membership);
}
