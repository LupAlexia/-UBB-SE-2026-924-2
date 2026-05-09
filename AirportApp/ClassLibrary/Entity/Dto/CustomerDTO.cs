namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record CustomerDTO(int Id, string Email, string? Phone, string Username, string PasswordHash, int? MembershipId);
}
