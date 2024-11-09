using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ProductsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.User).ToList();
            return View(products);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _context.Products
            .Include(p => p.User)
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
        return NotFound();
        }
        return View(product);
        }


        // GET: Products/Create
        public IActionResult Create()
        {
            // Sjekk om brukeren er innlogget
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
                
                // henter alle kategorier
                ViewBag.Categories = _context.Categories.ToList();
                return View();
        }



        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product, int[] selectedCategories)
        {
        if (ModelState.IsValid)
        {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        product.UserId = userId.Value;
        product.CreatedAt = DateTime.Now;

        // Legg til de valgte kategoriene
        product.ProductCategories = new List<ProductCategory>();
        foreach (var categoryId in selectedCategories)
        {
            product.ProductCategories.Add(new ProductCategory
            {
                CategoryId = categoryId,
                ProductId = product.Id
            });
        }

        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
        }      

        // Hvis modellen ikke er gyldig, send kategoriene tilbake til visningen
        ViewBag.Categories = _context.Categories.ToList();
        return View(product);
        }


        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
        return RedirectToAction("Login", "Account");
        }
        var product = _context.Products
        .Include(p => p.ProductCategories)
        .FirstOrDefault(p => p.Id == id && p.UserId == userId.Value);
        if (product == null)
        {
        return NotFound();
        }

        // Hent alle kategorier
        ViewBag.Categories = _context.Categories.ToList();
        // Hent valgte kategorier
        ViewBag.SelectedCategories = product.ProductCategories.Select(pc => pc.CategoryId).ToArray();

        return View(product);
}


        // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Product product, int[] selectedCategories)
    {
    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null)
    {
        return RedirectToAction("Login", "Account");
    }
    if (id != product.Id)
    {
        return NotFound();
    }
    if (ModelState.IsValid)
    {
        var existingProduct = _context.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefault(p => p.Id == id && p.UserId == userId.Value);
        if (existingProduct == null)
        {
            return NotFound();
        }

        // Oppdater egenskaper
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Calories = product.Calories;
        existingProduct.Fat = product.Fat;
        existingProduct.Carbohydrates = product.Carbohydrates;
        existingProduct.Protein = product.Protein;

        // Oppdater kategorier
        existingProduct.ProductCategories.Clear();
        foreach (var categoryId in selectedCategories)
        {
            existingProduct.ProductCategories.Add(new ProductCategory
            {
                CategoryId = categoryId,
                ProductId = existingProduct.Id
            });
        }

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    ViewBag.Categories = _context.Categories.ToList();
    ViewBag.SelectedCategories = selectedCategories;
    return View(product);
    }


        // GET: Products/Delete/5
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var product = _context.Products.FirstOrDefault(p => p.Id == id && p.UserId == userId.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var product = _context.Products.FirstOrDefault(p => p.Id == id && p.UserId == userId.Value);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
