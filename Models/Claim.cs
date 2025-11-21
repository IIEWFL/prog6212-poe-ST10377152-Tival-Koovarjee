using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Lecturer name is required")]
        [Display(Name = "Lecturer Name")]
        public string LecturerName { get; set; }

        [Required(ErrorMessage = "Claim month is required")]
        [Display(Name = "Month")]
        [DataType(DataType.Date)]
        public DateTime ClaimMonth { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(1, 200, ErrorMessage = "Hours worked must be between 1 and 200")]
        [Display(Name = "Hours Worked")]
        public int HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0.01, 1000, ErrorMessage = "Hourly rate must be positive")]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount (R)")]
        public decimal TotalAmount => HoursWorked * HourlyRate;

        [Display(Name = "Additional Notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        // Make these properties nullable since they're optional
        [Display(Name = "Supporting Document")]
        public string? FileName { get; set; }

        public string? FilePath { get; set; }
    }
}