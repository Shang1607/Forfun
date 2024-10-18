using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ApplicationDBContext _db;

        public CategoryController(ApplicationDBContext db)
        {
            _db = db;
        }   
        // GET: CategoryController
        public ActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        // get
        public ActionResult Create()
        {
            return View();
        }

        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString()){
                ModelState.AddModelError("name ", "The DisplayOrder cannot exactly match the Name");
            }
            if(ModelState.IsValid)
            {
                obj.Created = DateTime.Now;
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
        return View(obj);
        }

        // get
        public ActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDb = _db.Categories.Find(id);
            if(categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString()){
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name");
            }
            if(ModelState.IsValid)
            {
                
                _db.Categories.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        return View(obj);
        } 

        public ActionResult Delete(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDb = _db.Categories.Find(id);
            if(categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePOST(int? id)
        {
         
            var obj = _db.Categories.Find(id);
            if(obj == null) 
            {
                return NotFound();
            } 
                _db.Categories.Remove(obj);
                _db.SaveChanges();
                TempData["success"] = "Category deleted successfully";

                return RedirectToAction("Index");
            
        return View(obj);
        }     

    }

    
}