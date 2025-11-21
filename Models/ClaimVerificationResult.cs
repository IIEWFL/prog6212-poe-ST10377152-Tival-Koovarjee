namespace ContractMonthlyClaimSystem.Models
{
    public class ClaimVerificationResult
    {
        public int ClaimId { get; set; }
        public Claim OriginalClaim { get; set; }
        public string VerificationStatus { get; set; }
        public string RiskLevel { get; set; }
        public bool IsAutoApprovable { get; set; }
        public string VerificationFlags { get; set; }
        public string ReviewNotes { get; set; }

        // Helper properties for the view
        public bool HasFlags => !string.IsNullOrEmpty(VerificationFlags) && VerificationFlags != "None";
        public bool IsHighRisk => RiskLevel == "High";
        public bool IsMediumRisk => RiskLevel == "Medium";
    }
}