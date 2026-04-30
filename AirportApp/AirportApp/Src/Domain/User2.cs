namespace AirportApp.Src.Domain
{
    public class User2
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Membership? Membership { get; set; }

        public User2()
        {
        }

        public User2(string email, string? phone, string username, string passwordHash, Membership? membership)
        {
            Email = email;
            Phone = phone;
            Username = username;
            PasswordHash = passwordHash;
            Membership = membership;
        }

        public User2(int userId, string email, string? phone, string username, string passwordHash, Membership? membership)
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
