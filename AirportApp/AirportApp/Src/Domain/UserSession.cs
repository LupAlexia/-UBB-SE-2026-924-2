namespace AirportApp.Src.Domain
{
    public static class UserSession
    {
        public static User2? CurrentUser { get; set; }

#pragma warning disable SA1011
        public static object[]? PendingBookingParameters { get; set; }
#pragma warning restore SA1011
    }
}
