using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services
{
    public interface IClaimVerificationService
    {
        ClaimVerificationResult VerifyClaim(Claim claim);
        List<ClaimVerificationResult> VerifyClaims(List<Claim> claims);
    }

    public class ClaimVerificationService : IClaimVerificationService
    {
        // Hard-coded verification rules
        private const decimal HIGH_AMOUNT_THRESHOLD = 15000m;
        private const decimal SUSPICIOUS_RATE_THRESHOLD = 800m;
        private const int MAX_HOURS_PER_MONTH = 160;
        private const int UNUSUAL_HOURS_THRESHOLD = 60;

        public ClaimVerificationResult VerifyClaim(Claim claim)
        {
            var flags = new List<string>();
            var notes = new List<string>();
            var riskLevel = "Low";
            var autoApprovable = true;

            // Rule 1: High amount check
            if (claim.TotalAmount > HIGH_AMOUNT_THRESHOLD)
            {
                flags.Add("High Amount");
                notes.Add($"Claim amount R{claim.TotalAmount} exceeds R{HIGH_AMOUNT_THRESHOLD} threshold");
                riskLevel = "High";
                autoApprovable = false;
            }

            // Rule 2: Suspicious rate check
            if (claim.HourlyRate > SUSPICIOUS_RATE_THRESHOLD)
            {
                flags.Add("High Hourly Rate");
                notes.Add($"Hourly rate R{claim.HourlyRate} exceeds typical range");
                riskLevel = riskLevel == "Low" ? "Medium" : riskLevel;
                autoApprovable = false;
            }

            // Rule 3: Excessive hours check
            if (claim.HoursWorked > MAX_HOURS_PER_MONTH)
            {
                flags.Add("Excessive Hours");
                notes.Add($"{claim.HoursWorked} hours exceeds monthly maximum of {MAX_HOURS_PER_MONTH}");
                riskLevel = "High";
                autoApprovable = false;
            }
            else if (claim.HoursWorked > UNUSUAL_HOURS_THRESHOLD)
            {
                flags.Add("Unusual Hours");
                notes.Add($"{claim.HoursWorked} hours may require verification");
                riskLevel = riskLevel == "Low" ? "Medium" : riskLevel;
            }

            // Rule 4: Missing document check
            if (string.IsNullOrEmpty(claim.FileName))
            {
                flags.Add("No Supporting Document");
                notes.Add("Claim submitted without supporting documentation");
                riskLevel = riskLevel == "Low" ? "Medium" : riskLevel;
            }

            // Rule 5: Future date check
            if (claim.ClaimMonth > DateTime.Now)
            {
                flags.Add("Future Date");
                notes.Add("Claim month is in the future");
                riskLevel = "High";
                autoApprovable = false;
            }

            // Determine final status
            string status;
            if (flags.Count == 0)
            {
                status = "Auto-Approved";
                notes.Add("Claim meets all criteria for automatic approval");
            }
            else if (autoApprovable)
            {
                status = "Recommended for Approval";
                notes.Add("Minor flags detected but claim appears valid");
            }
            else
            {
                status = "Requires Manual Review";
                notes.Add("Significant verification flags detected");
            }

            return new ClaimVerificationResult
            {
                ClaimId = claim.ClaimId,
                OriginalClaim = claim,
                VerificationStatus = status,
                RiskLevel = riskLevel,
                IsAutoApprovable = autoApprovable,
                VerificationFlags = flags.Count > 0 ? string.Join(", ", flags) : "None",
                ReviewNotes = string.Join("; ", notes)
            };
        }

        public List<ClaimVerificationResult> VerifyClaims(List<Claim> claims)
        {
            var results = new List<ClaimVerificationResult>();

            foreach (var claim in claims)
            {
                results.Add(VerifyClaim(claim));
            }

            return results;
        }
    }
}