using System;
using System.Linq;
using System.Threading.Tasks; // For asynkrone operasjoner
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For asynkrone databaseoperasjoner
using Microsoft.Extensions.Logging; // For logging
using Test.Data;
using Test.Models;


namespace Test.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AccountController> _logger; // For logging

        public AccountController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment, ILogger<AccountController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // GET: Signup
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        // POST: Signup
        [HttpPost]
        public async Task<IActionResult> Signup(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sjekk om e-post allerede eksisterer
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "E-mail is already in use");
                        return View(model);
                    }

                    // Hasher passordet
                    var passwordHasher = new PasswordHasher<User>();
                    model.PasswordHash = passwordHasher.HashPassword(model, model.Password);

                    // Nullstill passordfeltet for sikkerhet
                    model.Password = null;

                    // Legg til ny bruker i databasen
                    model.CreatedAt = DateTime.Now;
                    await _context.Users.AddAsync(model);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Login");
                }
            catch (Exception ex)
        {
        _logger.LogError(ex, "An error occurred while signing up.");
        var errorMessage = ex.Message;
        if (ex.InnerException != null)
        {
        errorMessage += " Inner exception: " + ex.InnerException.Message;
        }
        ModelState.AddModelError("", $"An error occurred: {errorMessage}");
        }
            }
            else
            {
                // Logg valideringsfeil
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"Key: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ModelState.AddModelError("", "a validation error occurred. Please check your input and try again.");
            }

            return View(model);
        }

        // GET: EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            // Hent bruker-ID fra session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Bruker er ikke innlogget
                return RedirectToAction("Login");
            }

            try
            {
                // Hent brukeren fra databasen
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during profile editing.");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model, IFormFile ProfileImage)
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

            try
            {
                // Hent brukeren fra databasen
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
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
                        await ProfileImage.CopyToAsync(fileStream);
                    }

                    // Slett det gamle bildet hvis det finnes
                    if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                    {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                    System.IO.File.Delete(oldImagePath);
                    }
                    }
                    // Oppdater profilbilde-URL
                    user.ProfileImageUrl = $"/uploads/{fileName}";
                }

                // Lagre endringene
                await _context.SaveChangesAsync();

                // Omdiriger tilbake til profilsiden
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during profile editing");
                ModelState.AddModelError("", "An error occured during profile editing. Please try again later.");
                return View(model);
            }
        }

        // GET: ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
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

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user == null)
                {
                    return NotFound();
                }

                // Verifiser gammelt passord
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
                if (result != PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Password does not match old password.");
                    return View(model);
                }

                // Oppdater til nytt passord
                user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
                await _context.SaveChangesAsync();

                // Omdiriger eller vis suksessmelding
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during password change.");
                ModelState.AddModelError("", "An error occured during password change. Please try again later.");
                return View(model);
            }
        }

        // GET: Login
    [HttpGet]
    public IActionResult Login()
    {
    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId != null)
    {
        return RedirectToAction("UserProfile");
    }

    return View();
    }


        // POST: Login
 [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    // Sjekk om brukeren allerede er logget inn
    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId != null)
    {
        // Brukeren er allerede logget inn
        return RedirectToAction("AlreadyLoggedIn");
    }

    if (!ModelState.IsValid)
    {
        // Logg valideringsfeil
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                _logger.LogWarning($"Key: {state.Key}, Error: {error.ErrorMessage}");
            }
        }
        return View(model);
    }

    try
    {
        // Autentiser brukeren
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user != null)
        {
            // Verifiser passordet
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Success)
            {
                // Sett session-variabler
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("CompanyName", user.CompanyName ?? user.Email);

                return RedirectToAction("UserProfile");
            }
        }

        // Autentisering feilet
        ModelState.AddModelError("", "Ugyldig e-post eller passord.");
        return View(model);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "En feil oppstod under innlogging.");
        ModelState.AddModelError("", "En feil oppstod under innlogging. Vennligst prøv igjen senere.");
        return View(model);
    }
}


        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Tøm session
            HttpContext.Session.Clear();

            // Omdiriger til startsiden eller innloggingssiden
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AlreadyLoggedIn()
        {
        var companyName = HttpContext.Session.GetString("CompanyName") ?? "Bruker";
        ViewBag.CompanyName = companyName;
        return View();
        }

        // GET: UserProfile
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            // Hent bruker-ID fra session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Bruker er ikke innlogget
                return RedirectToAction("Login");
            }

            try
            {
                // Hent brukeren fra databasen
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user == null)
                {
                    return NotFound();
                }

                // Returner visningen med brukerdata
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during user profile retrieval.");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
