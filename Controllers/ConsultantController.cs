using GestionTM.Data;
using GestionTM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionTM.Controllers
{
    [Route("Consultant")]
    public class ConsultantController : Controller
    {
        private readonly AppDbContext _context;

        public ConsultantController(AppDbContext context)
        {
            _context = context;
        }

        // Home Action
        [HttpGet("Home")]
        public IActionResult Home()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Consultant" || userId == null)
            {
                return Unauthorized("Access Denied.");
            }

            var consultant = _context.Consultants.Find(userId);
            if (consultant == null)
            {
                return NotFound("Consultant not found.");
            }

            return View(consultant);
        }
        


        // Dashboard Action
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Consultant" || userId == null)
            {
                return Unauthorized("Access Denied.");
            }

            var consultant = _context.Consultants.Find(userId);
            if (consultant == null)
            {
                return NotFound("Consultant not found.");
            }

            var stats = new
            {
                Created = _context.Timesheets.Count(t => t.ConsultantId == userId && t.Status == "Created"),
                Submitted = _context.Timesheets.Count(t => t.ConsultantId == userId && t.Status == "Submitted"),
                Accepted = _context.Timesheets.Count(t => t.ConsultantId == userId && t.Status == "Accepted"),
                Refused = _context.Timesheets.Count(t => t.ConsultantId == userId && t.Status == "Refused")
            };

            ViewBag.Stats = stats;
            return View(consultant);
        }

        // GererTimesheets Action
        [HttpGet("GererTimesheets")]
        public IActionResult GererTimesheets()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || HttpContext.Session.GetString("UserRole") != "Consultant")
            {
                return Unauthorized("Access Denied.");
            }

            var timesheets = _context.Timesheets
                .Include(t => t.TimesheetLines)
                .Where(t => t.ConsultantId == userId)
                .ToList();

            return View(timesheets);
        }

        // CreerTimesheet Page
        [HttpGet("GererTimesheets/CreerTimesheet")]
        public IActionResult CreerTimesheet()
        {
            ViewBag.Validators = _context.Validateurs.Select(v => v.Email).ToList();
            ViewBag.Consultants = _context.Consultants.Select(c => c.Email).ToList();

            return View();
        }

        [HttpPost("GererTimesheets/CreerTimesheet")]
        [ValidateAntiForgeryToken]
        public IActionResult CreerTimesheet(string description, List<TimesheetLine> lines)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || HttpContext.Session.GetString("UserRole") != "Consultant")
            {
                return Unauthorized("Access Denied.");
            }

            if (string.IsNullOrEmpty(description) || lines == null || !lines.Any())
            {
                ViewBag.Message = "Please provide a description and at least one timesheet line.";
                ViewBag.Validators = _context.Validateurs.Select(v => v.Email).ToList();
                ViewBag.Consultants = _context.Consultants.Select(c => c.Email).ToList();
                return View();
            }

            var timesheet = new Timesheet
            {
                Description = description,
                ConsultantId = userId.Value,
                Status = "Created",
                TimesheetLines = lines
            };

            _context.Timesheets.Add(timesheet);
            _context.SaveChanges();

            return RedirectToAction("GererTimesheets");
        }


        // Delete TimesheetLine
        [HttpPost]
        public IActionResult DeleteTimesheetLine(int lineId)
        {
            var line = _context.TimesheetLines.Find(lineId);
            if (line != null)
            {
                _context.TimesheetLines.Remove(line);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost("GererTimesheets/SubmitTimesheet")]
        public IActionResult SubmitTimesheet(int timesheetId)
        {
            var timesheet = _context.Timesheets.Find(timesheetId);
            if (timesheet != null)
            {
                timesheet.Status = "Submitted";
                _context.SaveChanges();
                return RedirectToAction("GererTimesheets");
            }
            return NotFound();
        }
        [HttpGet("GererTimesheets/UpdateTimesheet")]
        public IActionResult UpdateTimesheet(int timesheetId)
        {
            var timesheet = _context.Timesheets
                .Include(t => t.TimesheetLines)
                .FirstOrDefault(t => t.Id == timesheetId);

            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        [HttpPost("GererTimesheets/UpdateTimesheet")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTimesheet(Timesheet updatedTimesheet)
        {
            var existingTimesheet = _context.Timesheets
                .Include(t => t.TimesheetLines)
                .FirstOrDefault(t => t.Id == updatedTimesheet.Id);

            if (existingTimesheet != null)
            {
                existingTimesheet.Description = updatedTimesheet.Description;
                existingTimesheet.TimesheetLines = updatedTimesheet.TimesheetLines;
                _context.SaveChanges();
                return RedirectToAction("GererTimesheets");
            }
            return NotFound();
        }
        [HttpPost("GererTimesheets/DeleteTimesheet")]
        public IActionResult DeleteTimesheet(int timesheetId)
        {
            var timesheet = _context.Timesheets
                .Include(t => t.TimesheetLines)
                .FirstOrDefault(t => t.Id == timesheetId);

            if (timesheet != null)
            {
                _context.TimesheetLines.RemoveRange(timesheet.TimesheetLines);
                _context.Timesheets.Remove(timesheet);
                _context.SaveChanges();
                return RedirectToAction("GererTimesheets");
            }
            return NotFound();
        }


    }
}