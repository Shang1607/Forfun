using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
     public class AccountController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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


          // GET: EditProfile
        [HttpGet]
        public IActionResult EditProfile()
        {
            // Hent bruker-ID fra session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Bruker er ikke innlogget
                return RedirectToAction("Login");
            }

            // Hent brukeren fra databasen
            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            // Opprett en ViewModel med eksisterende data
            var model = new EditProfileViewModel
            {
                CompanyName = user.CompanyName,
                Bio = user.Bio,
                ExistingProfileImageUrl = user.ProfileImageUrl
            };

            return View(model);
        }

        // POST: EditProfile
        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel model, IFormFile ProfileImage)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Hent bruker-ID fra session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            // Hent brukeren fra databasen
            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            // Oppdater brukerens informasjon
            user.CompanyName = model.CompanyName;
            user.Bio = model.Bio;

            // Håndter opplasting av profilbilde
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                // Generer unikt filnavn
                var fileName = $"{Guid.NewGuid()}_{ProfileImage.FileName}";
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads, fileName);

                // Sørg for at uploads-mappen eksisterer
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Lagre filen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(fileStream);
                }

                // Oppdater profilbilde-URL
                user.ProfileImageUrl = $"/uploads/{fileName}";
            }

            // Lagre endringene
            _context.SaveChanges();

            // Omdiriger tilbake til profilsiden
            return RedirectToAction("UserProfile");
        }
    



    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST: ChangePassword
    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Hent den innloggede brukeren
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
        // Bruker er ikke innlogget
        return RedirectToAction("Login");
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        // Verifiser gammelt passord
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
        if (result != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError("", "Gammelt passord er feil.");
            return View(model);
        }

        // Oppdater til nytt passord
        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
        _context.SaveChanges();

        // Omdiriger eller vis suksessmelding
        return RedirectToAction("UserProfile", new { id = user.Id });
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
        if (!ModelState.IsValid)
        {
        // Log validation errors
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
            }
        }
        return View(model);
        }

        // Authenticate the user
        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
        if (user != null)
        {
        // Verify the password
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Success)
        {
            // bruk av seassion 
            HttpContext.Session.SetInt32("UserId", user.Id);
            
            // TODO: Set authentication cookie or session
            return RedirectToAction("UserProfile", new { id = user.Id });
        }
        }

        // Authentication failed
        ModelState.AddModelError("", "Ugyldig e-post eller passord");
        return View(model);
        }       


        [HttpGet]
        public IActionResult Login()
        {
        return View(); // Dette vil returnere Views/Account/Login.cshtml
        }

        
    // GET: UserProfile
    [HttpGet]
    public IActionResult UserProfile()
    {
        // Hent bruker-ID fra session
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            // Bruker er ikke innlogget
            return RedirectToAction("Login");
        }

        // Hent brukeren fra databasen
        var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
        if (user == null)
        {
            return NotFound();
        }

        // Returner visningen med brukerdata
        return View(user);
    }
    
    }

}  