namespace PROG2112ClaimsPOE.Services
{
    public class VerificationResult
    {
        public bool IsApproved { get; set; }       // true when auto-approved
        public bool IsRejected { get; set; }       // true when auto-rejected
        public bool NeedsReview { get; set; }      // true when manager should review
        public List<string> Messages { get; set; } = new List<string>();
    }
}
