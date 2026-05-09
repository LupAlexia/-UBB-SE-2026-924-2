namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record MembershipAddonDiscountDTO(int membershipId, int addOnId, float discountPercentage, string addOnName);
}
