using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AirportApp.ClassLibrary.Entity.Domain.Message;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Users")]
    public class User : ISender
    {
        [Key]
        [Column("User_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Full_Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [Column("Email_Address")]
        public string EmailAddress { get; set; } = string.Empty;

        public User()
        {
        }
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