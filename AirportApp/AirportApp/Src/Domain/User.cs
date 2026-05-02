using AirportApp.Src.Domain;
using AirportApp.Src.Model.Message;

namespace AirportApp.Src.Model
{
    public class User : ISender
    {
        private int userId;
        private string fullName;
        private string emailAddress;
        public Membership? Membership { get; set; }

        public User(int userId, string fullName, string emailAddress, Membership? membership)
        {
            this.userId = userId;
            this.fullName = fullName;
            this.emailAddress = emailAddress;
            this.Membership = membership;
        }

        public int UserId => userId;

        public string RetrieveConfiguredDisplayFullNameForBot() => fullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => emailAddress;

        public int RetrieveUniqueDatabaseIdentifierForBot() => userId;
    }
}
