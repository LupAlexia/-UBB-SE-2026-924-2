namespace AirportApp.Src.Domain
{
    public class Customer
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
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
            UserId = userId;
            Email = email;
            Phone = phone;
            Username = username;
            PasswordHash = passwordHash;
            Membership = membership;
        }
    }
}
