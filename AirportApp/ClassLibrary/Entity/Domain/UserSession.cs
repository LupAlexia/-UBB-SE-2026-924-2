using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [NotMapped]
    public static class UserSession
    {
        public static Customer? CurrentUser { get; set; }

#pragma warning disable SA1011
        public static object[]? PendingBookingParameters { get; set; }
#pragma warning restore SA1011
    }
}
