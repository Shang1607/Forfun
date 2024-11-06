using Microsoft.AspNetCore.Identity;
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

        // Hasher passordet
        var passwordHasher = new PasswordHasher<User>();
        model.PasswordHash = passwordHasher.HashPassword(model, model.Password);

        // Nullstill passordfeltet for sikkerhet
        // model.Password = null;

        // Legg til ny bruker i databasen
        model.CreatedAt = DateTime.Now;
        _context.Users.Add(model);
        _context.SaveChanges();
        return RedirectToAction("Login");
    } else
    {
        // Iterate through ModelState errors and log them
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
            }
        }

        // Optionally, you can add a general error message
        ModelState.AddModelError("", "There were validation errors. Please review the form and correct them.");
    }

    return View(model);
}



[HttpPost]
public IActionResult Login(User model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    // Hent brukeren basert på e-post
    var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

    if (user != null)
    {
        // Verifiser passordet
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Success)
        {
            // Autentisering vellykket
            return RedirectToAction("UserProfile", new { id = user.Id });
        }
        else
        {
            // Feil passord
            ModelState.AddModelError("", "Ugyldig e-post eller passord");
            return View(model);
        }
    }
    else
    {
        // Bruker ikke funnet
        ModelState.AddModelError("", "Ugyldig e-post eller passord");
        return View(model);
    }
}

    [HttpGet]
    public IActionResult Login()
    {
    return View(); // Dette vil returnere Views/Account/Login.cshtml
    }

    [HttpGet]
    public IActionResult UserProfile(int id)
    {
    var user = _context.Users.FirstOrDefault(u => u.Id == id);
    if (user == null)
    {
        return NotFound(); // Returnerer 404 hvis brukeren ikke finnes
    }
    return View(user); // Sender brukerobjektet til visningen
    }
    }

}   