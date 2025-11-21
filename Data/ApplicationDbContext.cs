using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some initial data for testing - WITH ALL REQUIRED PROPERTIES
            modelBuilder.Entity<Claim>().HasData(
                new Claim
                {
                    ClaimId = 1,
                    LecturerName = "Dr. Sarah Smith",
                    ClaimMonth = new DateTime(2025, 2, 1),
                    HoursWorked = 40,
                    HourlyRate = 600,
                    Notes = "Regular teaching hours for February",
                    Status = "Approved",
                    SubmissionDate = new DateTime(2025, 2, 28),
                    FileName = "timesheet_feb.pdf",
                    FilePath = "uploads/timesheet_feb.pdf"  // Added this
                },
                new Claim
                {
                    ClaimId = 2,
                    LecturerName = "Prof. John Davis",
                    ClaimMonth = new DateTime(2025, 3, 1),
                    HoursWorked = 35,
                    HourlyRate = 650,
                    Notes = "March lecture hours",
                    Status = "Pending",
                    SubmissionDate = new DateTime(2025, 3, 15),
                    FileName = "march_hours.docx",
                    FilePath = "uploads/march_hours.docx"  // Added this
                }
            );
        }
    }
}