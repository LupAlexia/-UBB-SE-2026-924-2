using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Senders")]
    public abstract class Sender : ISender
    {
        [Key]
        [Column("Sender_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Full_Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("Email_Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [Column("Discriminator")]
        public string Discriminator { get; set; } = string.Empty;

        protected Sender()
        {
        }

        protected Sender(int id, string fullName, string emailAddress)
        {
            Id = id;
            FullName = fullName;
            EmailAddress = emailAddress;
        }

        public abstract int RetrieveUniqueDatabaseIdentifierForBot();
        public abstract string RetrieveConfiguredDisplayFullNameForBot();
        public abstract string RetrieveConfiguredEmailAddressForBotContact();
    }
}
