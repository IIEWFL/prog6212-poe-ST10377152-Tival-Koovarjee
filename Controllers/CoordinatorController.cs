using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class CoordinatorController : Controller
    {
        // GET: /Coordinator/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}