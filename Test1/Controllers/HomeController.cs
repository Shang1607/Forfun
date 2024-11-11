using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

[HttpGet]
public async Task<IActionResult> Search(string query)
{
    if (string.IsNullOrWhiteSpace(query))
    {
        ViewBag.Message = "search for the product youre looking for!";
        return View(new List<Product>());
    }

    try
    {
        var products = await _context.Products
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
            .ToListAsync();

        ViewBag.Query = query;
        return View(products);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "there was an error searching for products");
        // Vis en feilmelding til brukeren eller omdiriger til en feilside
        return RedirectToAction("Error");
    }
}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
