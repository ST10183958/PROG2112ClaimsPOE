using System;


namespace PROG2112ClaimsPOE.Models
{
    public class ApprovalLog
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string Actor { get; set; }        // username or system
        public string Action { get; set; }       // "Verified", "AutoApproved", "Rejected", "ManagerApproved"
        public string Notes { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
