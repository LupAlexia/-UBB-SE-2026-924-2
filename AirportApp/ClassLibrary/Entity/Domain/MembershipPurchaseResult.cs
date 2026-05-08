using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    /// <summary>
    /// Result of attempting a membership purchase.
    /// Used by MembershipService.PurchaseMembership to return
    /// success/failure with a message, so the ViewModel doesn't
    /// need try/catch or session management logic.
    /// </summary>
    ///
    [NotMapped]
    public class MembershipPurchaseResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
