namespace AirportApp.Mvc.Models.Customer
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string Username { get; set; } = string.Empty;

        public int? MembershipId { get; set; }

        public string? MembershipName { get; set; }
    }
}
