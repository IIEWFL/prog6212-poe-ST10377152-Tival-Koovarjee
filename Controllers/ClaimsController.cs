using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Services;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IClaimVerificationService _verificationService;

        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment environment, IClaimVerificationService verificationService)
        {
            _context = context;
            _environment = environment;
            _verificationService = verificationService;
        }

        // GET: Claims for Lecturers
        public async Task<IActionResult> Index()
        {
            var claims = await _context.Claims.ToListAsync();
            return View(claims);
        }

        // GET: Claims/Submit
        public IActionResult Submit()
        {
            return View();
        }

        // GET: Claims/VerificationReport - View automated verification results
        public async Task<IActionResult> VerificationReport()
        {
            var allClaims = await _context.Claims
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            var verificationResults = _verificationService.VerifyClaims(allClaims);

            var report = new VerificationReportViewModel
            {
                TotalClaims = allClaims.Count,
                AutoApprovedClaims = verificationResults.Count(v => v.VerificationStatus == "Auto-Approved"),
                ManualReviewClaims = verificationResults.Count(v => v.VerificationStatus == "Requires Manual Review"),
                HighRiskClaims = verificationResults.Count(v => v.RiskLevel == "High"),
                VerificationResults = verificationResults
            };

            return View(report);
        }

        // POST: Claims/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Claim claim, IFormFile supportingDocument)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload (optional)
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(supportingDocument.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await supportingDocument.CopyToAsync(fileStream);
                    }

                    claim.FileName = supportingDocument.FileName;
                    claim.FilePath = uniqueFileName;
                }

                claim.Status = "Pending";
                claim.SubmissionDate = DateTime.Now;

                _context.Add(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        // GET: Claims/Review - HARD-CODED AUTOMATION
        public async Task<IActionResult> Review()
        {
            var pendingClaims = await _context.Claims
                .Where(c => c.Status == "Pending")
                .ToListAsync();

            // HARD-CODED AUTOMATION: Run verification on all pending claims
            var verificationResults = _verificationService.VerifyClaims(pendingClaims);

            // HARD-CODED AUTOMATION: Auto-approve claims that pass all checks
            var autoApprovedClaims = new List<Claim>();
            foreach (var result in verificationResults.Where(r => r.VerificationStatus == "Auto-Approved"))
            {
                result.OriginalClaim.Status = "Approved";
                autoApprovedClaims.Add(result.OriginalClaim);
            }

            if (autoApprovedClaims.Any())
            {
                _context.Claims.UpdateRange(autoApprovedClaims);
                await _context.SaveChangesAsync();
                TempData["AutoApprovedMessage"] = $"{autoApprovedClaims.Count} claims were automatically approved.";

                // Refresh the list after auto-approval
                pendingClaims = await _context.Claims
                    .Where(c => c.Status == "Pending")
                    .ToListAsync();
                verificationResults = _verificationService.VerifyClaims(pendingClaims);
            }

            var viewModel = new ReviewViewModel
            {
                VerificationResults = verificationResults
            };

            return View(viewModel);
        }

        // POST: Claims/BulkApprove - HARD-CODED BULK APPROVAL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkApprove(int[] claimIds)
        {
            if (claimIds == null || claimIds.Length == 0)
            {
                TempData["ErrorMessage"] = "No claims selected for approval.";
                return RedirectToAction(nameof(Review));
            }

            var claimsToApprove = await _context.Claims
                .Where(c => claimIds.Contains(c.ClaimId) && c.Status == "Pending")
                .ToListAsync();

            // HARD-CODED: Verify claims and only approve auto-approvable ones
            var verificationResults = _verificationService.VerifyClaims(claimsToApprove);
            var approvableClaims = verificationResults
                .Where(v => v.IsAutoApprovable)
                .Select(v => v.OriginalClaim)
                .ToList();

            foreach (var claim in approvableClaims)
            {
                claim.Status = "Approved";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{approvableClaims.Count} claims were approved in bulk.";
            return RedirectToAction(nameof(Review));
        }

        // POST: Claims/AutoVerifyAll - HARD-CODED VERIFICATION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoVerifyAll()
        {
            var pendingClaims = await _context.Claims
                .Where(c => c.Status == "Pending")
                .ToListAsync();

            // HARD-CODED: Run verification and auto-approve
            var verificationResults = _verificationService.VerifyClaims(pendingClaims);
            var autoApprovedClaims = verificationResults
                .Where(r => r.VerificationStatus == "Auto-Approved")
                .Select(r => r.OriginalClaim)
                .ToList();

            foreach (var claim in autoApprovedClaims)
            {
                claim.Status = "Approved";
            }

            if (autoApprovedClaims.Any())
            {
                _context.Claims.UpdateRange(autoApprovedClaims);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Automated verification completed. {autoApprovedClaims.Count} claims auto-approved.";
            return RedirectToAction(nameof(Review));
        }

        // POST: Claims/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim approved successfully!";
            return RedirectToAction(nameof(Review));
        }

        // POST: Claims/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim rejected!";
            return RedirectToAction(nameof(Review));
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var claim = await _context.Claims.FirstOrDefaultAsync(m => m.ClaimId == id);
            if (claim == null) return NotFound();

            return View(claim);
        }

        // GET: Claims/Download/5
        public IActionResult Download(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null || string.IsNullOrEmpty(claim.FilePath)) return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, "uploads", claim.FilePath);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", claim.FileName);
        }
    }
}