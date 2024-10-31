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
  public IActionResult Signup([FromBody]User model)
       {
        
           Console.WriteLine($"Model: {model}");
           
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
       
               // Legg til ny bruker i databasen
               _context.Users.Add(model);
               _context.SaveChanges();
               return RedirectToAction("Login");
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

    // Authenticate the user
    var authenticatedUser = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

    if (authenticatedUser != null)
    {
        return RedirectToAction("UserProfile", new { id = authenticatedUser.Id });
    }
    else
    {
        // Return an error message
        ModelState.AddModelError("", "Invalid email or password");
        return View();
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




   