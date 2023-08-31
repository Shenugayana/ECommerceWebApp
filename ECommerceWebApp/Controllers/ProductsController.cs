using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models.Domain;
using ECommerceWebApp.Attributes;

namespace ECommerceWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DbModel _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(DbModel context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            // Query to retrieve products and include associated category
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Filtering based on search string
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Category.Name.Contains(searchString));
            }

            // Sorting logic based on sortOrder parameter
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "rating_desc":
                    query = query.OrderByDescending(p => p.Rating);
                    break;
                case "rating_asc":
                    query = query.OrderBy(p => p.Rating);
                    break;
                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            var products = await query.ToListAsync();
            return View(products);
        }

        [AdminAccess]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile? fImage, IFormFile? sImage, IFormFile? tImage, [Bind("Id,Name,Price,Stock,Description,Rating,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (fImage != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;

                    // Generate a unique prefix for filenames
                    string filePrefix = $"{product.Id}_{product.Name.Replace(" ", "_")}";

                    // Upload First Image
                    if (fImage != null)
                    {
                        string fImageFileName = $"{filePrefix}_FImage{Path.GetExtension(fImage.FileName)}";
                        string fImagePath = Path.Combine(wwwRootPath, "image");
                        using (var fFileStream = new FileStream(Path.Combine(fImagePath, fImageFileName), FileMode.Create))
                        {
                            fImage.CopyTo(fFileStream);
                        }
                        product.FImage = @"\image\" + fImageFileName;
                    }

                    // Upload Second Image
                    if (sImage != null)
                    {
                        string sImageFileName = $"{filePrefix}_SImage{Path.GetExtension(sImage.FileName)}";
                        string sImagePath = Path.Combine(wwwRootPath, "image");
                        using (var sFileStream = new FileStream(Path.Combine(sImagePath, sImageFileName), FileMode.Create))
                        {
                            sImage.CopyTo(sFileStream);
                        }
                        product.SImage = @"\image\" + sImageFileName;
                    }

                    // Upload Third Image
                    if (tImage != null)
                    {
                        string tImageFileName = $"{filePrefix}_TImage{Path.GetExtension(tImage.FileName)}";
                        string tImagePath = Path.Combine(wwwRootPath, "image");
                        using (var tFileStream = new FileStream(Path.Combine(tImagePath, tImageFileName), FileMode.Create))
                        {
                            tImage.CopyTo(tFileStream);
                        }
                        product.TImage = @"\image\" + tImageFileName;
                    }
                }

                _context.Add(product);
                TempData["successmsg"] = "Product added successfully!!";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Name", "Name", product.CategoryId);
            return View(product);
        }

        [AdminAccess]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile? fImage, IFormFile? sImage, IFormFile? tImage, [Bind("Id,Name,Price,Description,Rating,Stock,CategoryId,FImage,SImage,TImage")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string filePrefix = $"{product.Id}_{product.Name.Replace(" ", "_")}";

                // Replace First Image
                if (fImage != null)
                {
                    string fImageFileName = $"{filePrefix}_FImage{Path.GetExtension(fImage.FileName)}";
                    string fImagePath = Path.Combine(wwwRootPath, "image");
                    string fImageFilePath = Path.Combine(fImagePath, fImageFileName);

                    if (!string.IsNullOrEmpty(product.FImage) && System.IO.File.Exists(fImageFilePath))
                    {
                        System.IO.File.Delete(fImageFilePath);
                    }

                    using (var fFileStream = new FileStream(fImageFilePath, FileMode.Create))
                    {
                        fImage.CopyTo(fFileStream);
                    }
                    product.FImage = @"\image\" + fImageFileName;
                }
                else
                {
                    // Retain existing image path if no new image is provided
                    product.FImage = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == product.Id)?.FImage;
                }

                // Replace Second Image
                if (sImage != null)
                {
                    string sImageFileName = $"{filePrefix}_SImage{Path.GetExtension(sImage.FileName)}";
                    string sImagePath = Path.Combine(wwwRootPath, "image");
                    string sImageFilePath = Path.Combine(sImagePath, sImageFileName);

                    if (!string.IsNullOrEmpty(product.SImage) && System.IO.File.Exists(sImageFilePath))
                    {
                        System.IO.File.Delete(sImageFilePath);
                    }

                    using (var sFileStream = new FileStream(sImageFilePath, FileMode.Create))
                    {
                        sImage.CopyTo(sFileStream);
                    }
                    product.SImage = @"\image\" + sImageFileName;
                }
                else
                {
                    // Retain existing image path if no new image is provided
                    product.SImage = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == product.Id)?.SImage;
                }

                // Replace Third Image
                if (tImage != null)
                {
                    string tImageFileName = $"{filePrefix}_TImage{Path.GetExtension(tImage.FileName)}";
                    string tImagePath = Path.Combine(wwwRootPath, "image");
                    string tImageFilePath = Path.Combine(tImagePath, tImageFileName);

                    if (!string.IsNullOrEmpty(product.TImage) && System.IO.File.Exists(tImageFilePath))
                    {
                        System.IO.File.Delete(tImageFilePath);
                    }

                    using (var tFileStream = new FileStream(tImageFilePath, FileMode.Create))
                    {
                        tImage.CopyTo(tFileStream);
                    }
                    product.TImage = @"\image\" + tImageFileName;
                }
                else
                {
                    // Retain existing image path if no new image is provided
                    product.TImage = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == product.Id)?.TImage;
                }

                _context.Update(product);
                TempData["successmsg"] = "Product updated successfully!!";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [AdminAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'DbModel.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                TempData["successmsg"] = "Product deleted successfully!!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
