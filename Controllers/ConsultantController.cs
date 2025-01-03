using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTM.Data;
using GestionTM.Models;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: Timesheet/Home
        [HttpGet("Home")]
        public IActionResult Home()
        {
            return View(); 
        }

        // GET: Timesheet/Dashboard
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TimesheetsSubmited = await _context.Timesheets.CountAsync(t => t.Status == "Submitted");
            ViewBag.TimesheetsCreated = await _context.Timesheets.CountAsync();
            ViewBag.TimesheetsAccepted = await _context.Timesheets.CountAsync(t => t.Status == "Accepted");
            ViewBag.TimesheetsRefused = await _context.Timesheets.CountAsync(t => t.Status == "Refused");
            return View();
        }

        // GET: Consultant/GererTimesheets
        [HttpGet("GererTimesheets")]
        public async Task<IActionResult> GererTimesheets()
        {
            var timesheets = await _context.Timesheets
                .Include(t => t.Consultant)
                .Include(t => t.TimesheetLines)
                .ToListAsync();
            return View(timesheets);
        }
        

        // GET: Consultant/GererTimesheets/CreerTimesheet
        [HttpGet("GererTimesheets/CreerTimesheet")]
        public async Task<IActionResult> CreerTimesheet()
        {
            var consultants = await _context.Consultants.ToListAsync();
            var validateurs = await _context.Validateurs.ToListAsync();

            if (consultants == null || !consultants.Any())
            {
                Console.WriteLine("Aucun consultant trouvé.");
            }

            if (validateurs == null || !validateurs.Any())
            {
                Console.WriteLine("Aucun validateur trouvé.");
            }

            ViewBag.Consultants = consultants.Select(c => new { c.Id, c.Email }).ToList();
            ViewBag.Validateurs = validateurs.Select(v => new { v.Id, v.Email }).ToList();

            return View();
        }



        // POST: Consultant/GererTimesheets/CreerTimesheet
        [HttpPost("GererTimesheets/CreerTimesheet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreerTimesheet([Bind("Description,Status,ConsultantId")] Timesheet timesheet, [FromForm] List<TimesheetLine> timesheetLines)
        {
            if (ModelState.IsValid)
            {
                timesheet.TimesheetLines = timesheetLines;
                _context.Timesheets.Add(timesheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GererTimesheets));
            }
            ViewBag.Timesheets = await _context.Timesheets.ToListAsync();
            return View(timesheet);
        }
        

        // GET: Consultant/GererTimesheets/ModifierTimesheet
        [HttpGet("GererTimesheets/ModifierTimesheet")]
        public async Task<IActionResult> ModifierTimesheet(int id)
        {
            var timesheet = await _context.Timesheets
                .Include(t => t.TimesheetLines)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (timesheet == null)
            {
                return NotFound();
            }

            ViewBag.Consultants = await _context.Consultants.ToListAsync();
            return View(timesheet);
        }

        // POST: Consultant/GererTimesheets/UpdateTimesheet
        [HttpPost("GererTimesheets/UpdateTimesheet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTimesheet([Bind("Id,Description,Status,ConsultantId")] Timesheet timesheet, [FromForm] List<TimesheetLine> timesheetLines)
        {
            var existingTimesheet = await _context.Timesheets
                .Include(t => t.TimesheetLines)
                .FirstOrDefaultAsync(t => t.Id == timesheet.Id);

            if (existingTimesheet == null)
            {
                return NotFound();
            }

            existingTimesheet.Description = timesheet.Description;
            existingTimesheet.Status = timesheet.Status;
            existingTimesheet.ConsultantId = timesheet.ConsultantId;

            // Update TimesheetLines
            _context.TimesheetLines.RemoveRange(existingTimesheet.TimesheetLines);
            existingTimesheet.TimesheetLines = timesheetLines;

            _context.Update(existingTimesheet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GererTimesheets));
        }

        // POST: Consultant/GererTimesheets/DeleteTimesheet
        [HttpPost("GererTimesheets/DeleteTimesheet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTimesheet(int id)
        {
            var timesheet = await _context.Timesheets
                .Include(t => t.TimesheetLines)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (timesheet == null)
            {
                return NotFound();
            }

            _context.TimesheetLines.RemoveRange(timesheet.TimesheetLines);
            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GererTimesheets));
        }
    }
}
