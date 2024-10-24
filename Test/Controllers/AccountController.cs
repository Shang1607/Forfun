using Microsoft.AspNetCore.Mvc;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AccountController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Signup
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        // POST: Signup
        [HttpPost]
        public IActionResult Signup(User model)
        {
            if (ModelState.IsValid)
            {
                // Sjekk om e-post allerede eksisterer
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "E-post allerede i bruk");
                    return View(model);
                }

                // Legg til ny bruker i databasen
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
    public IActionResult Login()
    {
    return View(); // Dette vil returnere Views/Account/Login.cshtml
    }

    }
}
