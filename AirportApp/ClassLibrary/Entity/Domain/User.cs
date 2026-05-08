using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Users")]
    public class User : Sender
    {
        // Id, FullName, EmailAddress inherited from Sender
        public User()
        {
            Discriminator = "User";
        }
        public User(int id, string fullName, string emailAddress) : base(id, fullName, emailAddress)
        {
            Discriminator = "User";
        }

        public int UserId => Id;

        public override string RetrieveConfiguredDisplayFullNameForBot() => FullName;
        public override string RetrieveConfiguredEmailAddressForBotContact() => EmailAddress;
        public override int RetrieveUniqueDatabaseIdentifierForBot() => Id;
    }
}