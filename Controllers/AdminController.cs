using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTM.Data;
using GestionTM.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GestionTM.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Home
        [HttpGet("Home")]
        public IActionResult Home()
        {
            return View(); // Ensure this returns the `Home.cshtml` in Views/Admin
        }


        // GET: Admin/Dashboard
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.ConsultantsCount = await _context.Consultants.CountAsync();
            ViewBag.ValidateursCount = await _context.Validateurs.CountAsync();
            return View();
        }

        // GET: Admin/GererConsultants
        [HttpGet("GererConsultants")]
        public async Task<IActionResult> GererConsultants()
        {
            var consultants = await _context.Consultants
                .Include(c => c.Validateur)
                .ToListAsync();
            return View(consultants);
        }

        // GET: Admin/GererConsultants/CreerConsultant
        [HttpGet("GererConsultants/CreerConsultant")]
        public async Task<IActionResult> CreerConsultant()
        {
            ViewBag.Validateurs = await _context.Validateurs.ToListAsync();
            return View();
        }

        // POST: Admin/GererConsultants/CreerConsultant
        [HttpPost("GererConsultants/CreerConsultant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreerConsultant([Bind("Email,Password,PhoneNumber,ValidateurId")] Consultant consultant)
        {
            if (!consultant.Password.StartsWith("CC"))
            {
                ViewBag.Message = "Password must start with 'CC'.";
                ViewBag.Validateurs = await _context.Validateurs.ToListAsync();
                return View();
            }

            _context.Consultants.Add(consultant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GererConsultants));
        }

        // GET: Admin/GererConsultants/ModifierConsultant
        [HttpGet("GererConsultants/ModifierConsultant")]
        public async Task<IActionResult> ModifierConsultant(int id)
        {
            var consultant = await _context.Consultants.FindAsync(id);
            if (consultant == null)
            {
                return NotFound();
            }

            var validateurs = await _context.Validateurs.ToListAsync();
            var viewModel = new ModifierConsultantViewModel
            {
                Consultant = consultant,
                ValidatorSelectList = validateurs
                    .Select(v => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = v.Id.ToString(),
                        Text = v.Email,
                        Selected = v.Id == consultant.ValidateurId
                    }).ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/GererConsultants/UpdateConsultant
        [HttpPost("GererConsultants/UpdateConsultant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateConsultant([Bind("Id,Email,PhoneNumber,ValidateurId")] Consultant consultant)
        {
            var existingConsultant = await _context.Consultants.FindAsync(consultant.Id);
            if (existingConsultant == null)
            {
                return NotFound();
            }

            existingConsultant.Email = consultant.Email;
            existingConsultant.PhoneNumber = consultant.PhoneNumber;
            existingConsultant.ValidateurId = consultant.ValidateurId;

            _context.Update(existingConsultant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GererConsultants));
        }

        // POST: Admin/GererConsultants/DeleteConsultant
        [HttpPost("GererConsultants/DeleteConsultant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConsultant(int id)
        {
            var consultant = await _context.Consultants.FindAsync(id);
            if (consultant == null)
            {
                return NotFound();
            }

            _context.Consultants.Remove(consultant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GererConsultants));
        }

        // GET: Admin/GererValidateurs
        [HttpGet("GererValidateurs")]
        public async Task<IActionResult> GererValidateurs()
        {
            var validateurs = await _context.Validateurs
                .Include(v => v.Consultants)
                .ToListAsync();

            return View(validateurs);
        }

        // GET: Admin/GererValidateurs/CreerValidateur
        [HttpGet("GererValidateurs/CreerValidateur")]
        public async Task<IActionResult> CreerValidateur()
        {
            ViewBag.Consultants = await _context.Consultants
                .Where(c => c.ValidateurId == null)
                .ToListAsync();
            return View();
        }

        // POST: Admin/GererValidateurs/CreerValidateur
        [HttpPost("GererValidateurs/CreerValidateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreerValidateur(Validateur validateur, int[] consultantIds)
        {
            if (!validateur.Password.StartsWith("VV"))
            {
                ViewBag.Message = "Password must start with 'VV'.";
                ViewBag.Consultants = await _context.Consultants
                    .Where(c => c.ValidateurId == null)
                    .ToListAsync();
                return View();
            }

            _context.Validateurs.Add(validateur);
            await _context.SaveChangesAsync();

            // Update consultants with the new validateur
            foreach (var consultantId in consultantIds)
            {
                var consultant = await _context.Consultants.FindAsync(consultantId);
                if (consultant != null)
                {
                    consultant.ValidateurId = validateur.Id;
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GererValidateurs));
        }

        // GET: Admin/GererValidateurs/UpdateValidateur
        [HttpGet("GererValidateurs/UpdateValidateur")]
        public async Task<IActionResult> UpdateValidateur(int id)
        {
            var validateur = await _context.Validateurs
                .Include(v => v.Consultants)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (validateur == null)
            {
                return NotFound();
            }

            var availableConsultants = await _context.Consultants
                .Where(c => c.ValidateurId == null || c.ValidateurId == id)
                .ToListAsync();

            var viewModel = new UpdateValidateurViewModel
            {
                Validateur = validateur,
                ConsultantSelectList = availableConsultants.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Email,
                    Selected = validateur.Consultants.Any(vc => vc.Id == c.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/GererValidateurs/UpdateValidateur
        [HttpPost("GererValidateurs/UpdateValidateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateValidateur(Validateur validateur, int[] consultantIds)
        {
            var existingValidateur = await _context.Validateurs
                .Include(v => v.Consultants)
                .FirstOrDefaultAsync(v => v.Id == validateur.Id);

            if (existingValidateur == null)
            {
                return NotFound();
            }

            if (!validateur.Password.StartsWith("VV"))
            {
                ViewBag.Message = "Password must start with 'VV'.";
                return View(existingValidateur);
            }

            existingValidateur.Email = validateur.Email;
            existingValidateur.Password = validateur.Password;
            existingValidateur.PhoneNumber = validateur.PhoneNumber;

            // Clear existing consultant assignments
            foreach (var consultant in existingValidateur.Consultants)
            {
                consultant.ValidateurId = null;
            }

            // Assign new consultants
            foreach (var consultantId in consultantIds)
            {
                var consultant = await _context.Consultants.FindAsync(consultantId);
                if (consultant != null)
                {
                    consultant.ValidateurId = validateur.Id;
                }
            }

            _context.Update(existingValidateur);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GererValidateurs));
        }

        // POST: Admin/GererValidateurs/DeleteValidateur
        [HttpPost("GererValidateurs/DeleteValidateur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteValidateur(int id)
        {
            var validateur = await _context.Validateurs
                .Include(v => v.Consultants)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (validateur == null)
            {
                return NotFound();
            }

            foreach (var consultant in validateur.Consultants)
            {
                consultant.ValidateurId = null;
            }

            _context.Validateurs.Remove(validateur);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GererValidateurs));
        }


    }



}



