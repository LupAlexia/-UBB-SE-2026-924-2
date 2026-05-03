using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class User : ISender
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;

        public User() { }
        public User(int id, string fullName, string emailAddress)
        {
            Id = id;
            FullName = fullName;
            EmailAddress = emailAddress;
        }

        public int UserId => Id;

        public string RetrieveConfiguredDisplayFullNameForBot() => FullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => EmailAddress;

        public int RetrieveUniqueDatabaseIdentifierForBot() => Id;
    }
}