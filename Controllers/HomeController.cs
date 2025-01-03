using GestionTM.Data;
using GestionTM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GestionTM.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string email, string password, string phoneNumber)
        {
            // Check if user already exists in any table
            bool userExists = _context.Admins.Any(a => a.Email == email) ||
                              _context.Consultants.Any(c => c.Email == email) ||
                              _context.Validateurs.Any(v => v.Email == email);

            if (userExists)
            {
                ViewBag.Message = "You are already registered. Please log in.";
                return View("Index");
            }

            // Determine user type by password prefix
            if (password.StartsWith("AA"))
            {
                var admin = new Admin { Email = email, Password = password, PhoneNumber = phoneNumber };
                _context.Admins.Add(admin);
            }
            else if (password.StartsWith("CC"))
            {
                var consultant = new Consultant { Email = email, Password = password, PhoneNumber = phoneNumber };
                _context.Consultants.Add(consultant);
            }
            else if (password.StartsWith("VV"))
            {
                var validateur = new Validateur { Email = email, Password = password, PhoneNumber = phoneNumber };
                _context.Validateurs.Add(validateur);
            }
            else
            {
                ViewBag.Message = "Invalid password prefix. Registration failed.";
                return View("Index");
            }

            _context.SaveChanges();
            ViewBag.Message = "Registration successful! Please log in.";
            return View("Index");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (_context.Admins.Any(a => a.Email == email && a.Password == password))
            {
                return RedirectToAction("Home", "Admin");
            }
            if (_context.Consultants.Any(c => c.Email == email && c.Password == password))
            {
                return RedirectToAction("Home", "Consultant");
            }
            if (_context.Validateurs.Any(v => v.Email == email && v.Password == password))
            {
                return RedirectToAction("Home", "Validateur");
            }

            ViewBag.Message = "Invalid credentials. Please try again.";
            return View("Index");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }


    }
}


