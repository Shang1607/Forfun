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
        var model = new User();
        return View(model);  // Returnerer signup-skjemaet
        }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Signup(User model)
    {
    if (ModelState.IsValid)
    {
        try
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "E-post allerede i bruk");
                return View(model);
            }

            _context.Users.Add(model);

            _context.SaveChanges();  // Dette kan kaste unntak

            return RedirectToAction("UserProfile", new { id = model.Id });
        }
        catch (Exception ex)
        {
               // Logg hele unntaket, inkludert stack trace
        Console.WriteLine("Feil under lagring: " + ex.ToString());
    
         // Returner visningen med feil for debugging
        return View(model);  // Returner visningen i stedet for å vise en blank side
        }
    }

    return View(model);
}


        [HttpGet]
        public IActionResult UserProfile(int id)
        {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
        return NotFound(); // Hvis brukeren ikke finnes
        }
        return View(user); // Sender brukerobjektet til visningen
        }


        [HttpGet]
        public IActionResult Login()
        {
        return View(); // Dette vil returnere Views/Account/Login.cshtml
        }

        [HttpGet]
        public IActionResult EditProfile(int id)
        {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
        return NotFound();
        }
        return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User model)
        {
        if (ModelState.IsValid)
        {
        var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
        if (user == null)
        {
            return NotFound();
        }

        user.CompanyName = model.CompanyName;
        user.Bio = model.Bio;
        user.ProfileImageUrl = model.ProfileImageUrl;

        _context.SaveChanges();
        return RedirectToAction("UserProfile", new { id = user.Id });
        }
        return View(model);
        }

        
    }
}
