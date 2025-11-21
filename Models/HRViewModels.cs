using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Models
{
    public class HRDashboardViewModel
    {
        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int PendingClaims { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Claim> RecentClaims { get; set; } = new List<Claim>();
    }

    public class MonthlyReportViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalClaims { get; set; }
        public decimal ApprovedAmount { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }

    public class LecturerSummary
    {
        public string Name { get; set; }
        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime LastSubmission { get; set; }
    }
}