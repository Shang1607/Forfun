using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.User).ToListAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.User)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int[] selectedCategories, IFormFile ImageUrl)
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

        // Håndtere bildeopplasting
        if (ImageUrl != null && ImageUrl.Length > 0)
        {
            // Generer et unikt filnavn
            var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName);
            var extension = Path.GetExtension(ImageUrl.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

            // Angi banen der filen skal lagres
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, uniqueFileName);

            // Lagre filen
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageUrl.CopyTo(fileStream);
            }

            // Lagre filbanen i produktet
            product.ImageUrl = $"/uploads/{uniqueFileName}";
        }

                // Legg til de valgte kategoriene
                product.ProductCategories = new List<ProductCategory>();
                foreach (var categoryId in selectedCategories)
                {
                    product.ProductCategories.Add(new ProductCategory
                    {
                        CategoryId = categoryId
                    });
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategories = product.ProductCategories.Select(pc => pc.CategoryId).ToArray();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, int[] selectedCategories, IFormFile ImageUrl)
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
                var existingProduct = await _context.Products
                    .Include(p => p.ProductCategories)
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);
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

                // Håndtere bildeopplasting
        if (ImageUrl != null && ImageUrl.Length > 0)
        {
            // Slett eksisterende bilde hvis det finnes
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Generer et unikt filnavn
            var fileName = Path.GetFileNameWithoutExtension(ImageUrl.FileName);
            var extension = Path.GetExtension(ImageUrl.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

            // Angi banen der filen skal lagres
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, uniqueFileName);

            // Lagre filen
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageUrl.CopyTo(fileStream);
            }

            // Lagre filbanen i produktet
            existingProduct.ImageUrl = $"/uploads/{uniqueFileName}";
        }

                // Oppdater kategorier
                existingProduct.ProductCategories.Clear();
                foreach (var categoryId in selectedCategories)
                {
                    existingProduct.ProductCategories.Add(new ProductCategory
                    {
                        CategoryId = categoryId
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategories = selectedCategories;
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);

            if (product == null)
            {
                return NotFound();
            }

            // Slette bildet hvis det finnes
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
