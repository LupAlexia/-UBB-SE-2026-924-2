using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class User : ISender
    {
        private int userId;
        private string fullName;
        private string emailAddress;

        public User(int userId, string fullName, string emailAddress)
        {
            this.userId = userId;
            this.fullName = fullName;
            this.emailAddress = emailAddress;
        }

        public int UserId => userId;

        public string RetrieveConfiguredDisplayFullNameForBot() => fullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => emailAddress;

        public int RetrieveUniqueDatabaseIdentifierForBot() => userId;
    }
}