using GestionTM.Data;
using GestionTM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GestionTM.Controllers
{
    [Route("Validateur")]
    public class ValidateurController : Controller
    {
        private readonly AppDbContext _context;

        public ValidateurController(AppDbContext context)
        {
            _context = context;
        }

        // Home Action
        [HttpGet("Home")]
        public IActionResult Home()
        {
            return View();
        }

        // Dashboard Action
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? validateurId = HttpContext.Session.GetInt32("UserId");
            if (validateurId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var consultants = _context.Consultants
                .Where(c => c.ValidateurId == validateurId)
                .Select(c => c.Id)
                .ToList();

            var timesheets = _context.Timesheets
                .Where(t => consultants.Contains(t.ConsultantId))
                .ToList();

            var dashboardData = new
            {
                SubmittedCount = timesheets.Count(t => t.Status == "Submitted"),
                AcceptedCount = timesheets.Count(t => t.Status == "Accepted"),
                RefusedCount = timesheets.Count(t => t.Status == "Refused")
            };

            ViewBag.DashboardData = dashboardData;
            return View();
        }

        // GererTimesheets Action
        [HttpGet("GererTimesheets")]
        public IActionResult GererTimesheets()
        {
            int? validateurId = HttpContext.Session.GetInt32("UserId");
            if (validateurId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var timesheets = _context.Timesheets
                .Include(t => t.TimesheetLines)
                .Include(t => t.Consultant)
                .Where(t => t.Status == "Submitted" && t.Consultant.ValidateurId == validateurId)
                .ToList();

            return View(timesheets);
        }

        [HttpPost]
        [Route("Validateur/Accept/{id}")]
        public IActionResult Accept(int id)
        {
            var timesheet = _context.Timesheets.Find(id);
            if (timesheet != null && timesheet.Status == "Submitted")
            {
                timesheet.Status = "Accepted";
                _context.SaveChanges();
            }

            return RedirectToAction("GererTimesheets");
        }

        [HttpPost]
        [Route("Validateur/Refuse/{id}")]
        public IActionResult Refuse(int id)
        {
            var timesheet = _context.Timesheets.Find(id);
            if (timesheet != null && timesheet.Status == "Submitted")
            {
                timesheet.Status = "Refused";
                _context.SaveChanges();
            }

            return RedirectToAction("GererTimesheets");
        }
    }
}


