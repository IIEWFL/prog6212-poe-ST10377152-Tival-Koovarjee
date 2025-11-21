namespace ContractMonthlyClaimSystem.Models
{
    public class ReviewViewModel
    {
        public List<ClaimVerificationResult> VerificationResults { get; set; } = new List<ClaimVerificationResult>();

        // Calculated properties
        public int TotalClaims => VerificationResults.Count;
        public int AutoApprovedClaims => VerificationResults.Count(v => v.VerificationStatus == "Auto-Approved");
        public int ManualReviewClaims => VerificationResults.Count(v => v.VerificationStatus == "Requires Manual Review");
        public int HighRiskClaims => VerificationResults.Count(v => v.RiskLevel == "High");
        public int ReadyForBulkApproval => VerificationResults.Count(v => v.IsAutoApprovable && v.OriginalClaim.Status == "Pending");
    }
}