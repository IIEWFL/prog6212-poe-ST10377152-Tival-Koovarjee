using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HR Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var claims = await _context.Claims.ToListAsync();

                var viewModel = new HRDashboardViewModel
                {
                    TotalClaims = claims.Count,
                    ApprovedClaims = claims.Count(c => c.Status == "Approved"),
                    PendingClaims = claims.Count(c => c.Status == "Pending"),
                    TotalAmount = claims.Where(c => c.Status == "Approved").Sum(c => c.TotalAmount),
                    RecentClaims = claims.OrderByDescending(c => c.SubmissionDate).Take(10).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return View(new HRDashboardViewModel());
            }
        }

        // GET: Generate Monthly Report
        public async Task<IActionResult> GenerateReport(int? month, int? year)
        {
            try
            {
                var targetMonth = month ?? DateTime.Now.Month;
                var targetYear = year ?? DateTime.Now.Year;

                // Get all claims first, then filter in memory
                var allClaims = await _context.Claims.ToListAsync();

                var claims = allClaims
                    .Where(c => c.ClaimMonth.Month == targetMonth && c.ClaimMonth.Year == targetYear)
                    .ToList();

                var report = new MonthlyReportViewModel
                {
                    Month = targetMonth,
                    Year = targetYear,
                    TotalClaims = claims.Count,
                    ApprovedAmount = claims.Where(c => c.Status == "Approved").Sum(c => c.TotalAmount),
                    Claims = claims
                };

                return View(report);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while generating the report.";
                return View(new MonthlyReportViewModel());
            }
        }

        // GET: Lecturer Management
        public async Task<IActionResult> LecturerManagement()
        {
            try
            {
                // Get all claims first, then perform grouping in memory
                var allClaims = await _context.Claims.ToListAsync();

                var lecturers = allClaims
                    .GroupBy(c => c.LecturerName)
                    .Select(g => new LecturerSummary
                    {
                        Name = g.Key,
                        TotalClaims = g.Count(),
                        ApprovedClaims = g.Count(c => c.Status == "Approved"),
                        TotalAmount = g.Where(c => c.Status == "Approved").Sum(c => c.TotalAmount),
                        LastSubmission = g.Max(c => c.SubmissionDate)
                    })
                    .OrderByDescending(l => l.TotalAmount)
                    .ToList();

                return View(lecturers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading lecturer data.";
                return View(new List<LecturerSummary>());
            }
        }
    }
}