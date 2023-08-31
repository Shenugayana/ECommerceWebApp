using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models.Domain;

namespace ECommerceWebApp.Controllers
{
    public class CartsController : Controller
    {
        private readonly DbModel _context;
        public CartsController(DbModel context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId != null)
            {
                var dbModel = _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User)
                .Where(c => c.UserId == userId);
                return View(await dbModel.ToListAsync());
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, int qty, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var uId = HttpContext.Session.GetInt32("userId");
                if (uId != null)
                {
                    int userId = (int)uId;

                    var existingItem = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == id);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += qty;
                    }
                    else
                    {
                        Cart cart = new Cart
                        {
                            Quantity = qty,
                            ProductId = id,
                            UserId = userId
                        };
                        _context.Add(cart);
                    }

                    await _context.SaveChangesAsync();
                    TempData["successmsg"] = "Product successfully added to cart!!";
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    TempData["returnUrl"] = returnUrl;
                    return RedirectToAction("Login", "Users");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,Quantity")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", cart.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            return View(cart);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'DbModel.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            TempData["successmsg"] = "Product successfully removed from the cart!!";
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
          return (_context.Carts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
