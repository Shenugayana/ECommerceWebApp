using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models.Domain;

namespace ECommerceWebApp.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly DbModel _context;

        public OrderItemsController(DbModel context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            var dbModel = _context.OrderItems.Include(o => o.Order).Include(o => o.Product).Where(o => o.OrderId == id);
            return View(await dbModel.ToListAsync());
        }

        public async Task<IActionResult> Create(int method, OrderItems orderItems)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null || _context.Users == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                var cartItems = _context.Carts.Include(cart => cart.Product).Where(cart => cart.UserId == userId).ToList();
                if (cartItems.Count == 0)
                {
                    return RedirectToAction("Index", "Carts");
                }

                var order = new Order
                {
                    UserId = (int)userId,
                    Total = 0,
                    PaymentId = method,
                    Date = DateTime.Now
                };

                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItems
                    {
                        Order = order,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Date = DateTime.Now
                    };

                    // Retrieve the product based on ProductId
                    var product = _context.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                    if (product != null)
                    {
                        // Reduce the product's stock by the cart item's quantity
                        product.Stock -= cartItem.Quantity;

                        // Update the product in the context
                        _context.Products.Update(product);
                    }

                    order.Total += cartItem.Product.Price * cartItem.Quantity;

                    _context.OrderItems.Add(orderItem);
                    _context.Carts.Remove(cartItem);
                    TempData["successmsg"] = "Order placed successfully!!";
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Orders");
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItems = await _context.OrderItems.FindAsync(id);
            if (orderItems == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItems.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderItems.ProductId);
            return View(orderItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,ProductId,Date")] OrderItems orderItems)
        {
            if (id != orderItems.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItems);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemsExists(orderItems.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItems.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderItems.ProductId);
            return View(orderItems);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItems = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItems == null)
            {
                return NotFound();
            }

            return View(orderItems);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItems = await _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItems == null)
            {
                return NotFound();
            }

            return View(orderItems);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderItems == null)
            {
                return Problem("Entity set 'DbModel.OrderItems'  is null.");
            }
            var orderItems = await _context.OrderItems.FindAsync(id);
            if (orderItems != null)
            {
                _context.OrderItems.Remove(orderItems);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemsExists(int id)
        {
          return (_context.OrderItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
