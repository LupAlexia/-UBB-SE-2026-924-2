namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public int? MembershipId { get; set; }
        public Membership? Membership { get; set; }

        public Customer()
        {
        }

        public Customer(string email, string? phone, string username, string passwordHash, Membership? membership)
        {
            Email = email;
            Phone = phone;
            Username = username;
            PasswordHash = passwordHash;
            Membership = membership;
        }

        public Customer(int userId, string email, string? phone, string username, string passwordHash, Membership? membership)
        {
            Id = userId;
            Email = email;
            Phone = phone;
            Username = username;
            PasswordHash = passwordHash;
            Membership = membership;
            MembershipId = membership?.Id;
        }
    }
}
