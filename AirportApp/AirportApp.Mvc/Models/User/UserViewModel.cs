namespace AirportApp.Mvc.Models.User
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;
    }
}
