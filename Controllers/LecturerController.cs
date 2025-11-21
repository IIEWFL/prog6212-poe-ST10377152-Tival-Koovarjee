using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class LecturerController : Controller
    {
        // GET: /Lecturer/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: /Lecturer/SubmitClaim
        public IActionResult SubmitClaim()
        {
            return View();
        }
    }
}