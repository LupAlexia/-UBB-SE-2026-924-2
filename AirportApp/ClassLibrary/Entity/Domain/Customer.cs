using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [Column("Customer_Id")]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("Phone")]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)] 
        [Column("Password_Hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("Membership_Id")]
        public int? MembershipId { get; set; }

        [ForeignKey(nameof(MembershipId))]
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
            MembershipId = membership?.Id;
        }

        public Customer(int userId, string email, string? phone, string username, string passwordHash, Membership? membership)
        {
            Id = userId;
            Email = email;
            Phone = phone;
            Username = username;
            PasswordHash = passwordHash;
            Membership = membership;
            MembershipId = membership?.Id;
        }
    }
}
