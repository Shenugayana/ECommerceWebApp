using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models.Domain;
using ECommerceWebApp.Attributes;

namespace ECommerceWebApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DbModel _context;

        public CategoriesController(DbModel context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'DbModel.Categories'  is null.");
        }

        [AdminAccess]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (_context.Categories.Any(c => c.Name == category.Name))
                {
                    TempData["errormsg"] = "This Category already exists!!";
                }
                else
                {
                    _context.Add(category);
                    TempData["successmsg"] = "Category added successfully!!";
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }

        [AdminAccess]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Categories.Any(c => c.Name == category.Name))
                    {
                        TempData["errormsg"] = "This Category already exists!!";
                    }
                    else
                    {
                        _context.Update(category);
                        TempData["successmsg"] = "Category updated successfully!!";
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [AdminAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string deleteOption)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (deleteOption == "deleteCategory")
            {
                // Set category ID of associated products to null
                foreach (var product in category.Products)
                {
                    product.CategoryId = null;
                }
            }
            else if (deleteOption == "deleteAll")
            {
                // Remove all associated products
                _context.Products.RemoveRange(category.Products);
            }

            // Remove the category
            _context.Categories.Remove(category);

            TempData["successmsg"] = "Category deleted successfully!!";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
