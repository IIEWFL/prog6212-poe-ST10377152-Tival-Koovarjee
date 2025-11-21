namespace ContractMonthlyClaimSystem.Models
{
    public class VerificationReportViewModel
    {
        public int TotalClaims { get; set; }
        public int AutoApprovedClaims { get; set; }
        public int ManualReviewClaims { get; set; }
        public int HighRiskClaims { get; set; }
        public List<ClaimVerificationResult> VerificationResults { get; set; } = new List<ClaimVerificationResult>();
    }
}