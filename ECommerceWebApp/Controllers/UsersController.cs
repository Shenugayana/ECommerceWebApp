using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models.Domain;
using ECommerceWebApp.Attributes;
using ECommerceWebApp.Models;

namespace ECommerceWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly DbModel _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsersController(DbModel context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AdminAccess]
        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                        View(await _context.Users.ToListAsync()) :
                        Problem("Entity set 'DbModel.Users'  is null.");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile? file, RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                bool emailExists = _context.Users.Any(u => u.Emailaddress == register.Emailaddress);

                if (emailExists)
                {
                    TempData["errormsg"] = "This email address already exists!";
                    return View(register);
                }

                var user = new User
                {
                    Emailaddress = register.Emailaddress,
                    First_Name = register.First_Name,
                    Last_Name = register.Last_Name,
                    Address = register.Address,
                    Contact_Number = register.Contact_Number,
                    Password = register.Password,
                    Date_of_Join = DateTime.Now
                };

                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = $"{register.Emailaddress.Replace("@", "_")}{Path.GetExtension(file.FileName)}";
                    string userImagePath = Path.Combine(wwwRootPath, "image");

                    using (var fileStream = new FileStream(Path.Combine(userImagePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    user.Image = @"\image\" + fileName;
                }

                _context.Users.Add(user);
                TempData["successmsg"] = "You are registered successfully!!";
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Users");
            }

            return View(register);
        }

        public async Task<IActionResult> Edit()
        {
            var id = HttpContext.Session.GetInt32("userId");
            if (id == null || _context.Users == null)
            {
                return RedirectToAction("Account", "Users");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile? file, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if the email address already exists
                bool emailExists = _context.Users.Any(u => u.Emailaddress == user.Emailaddress && u.Id != user.Id);

                if (emailExists)
                {
                    TempData["errormsg"] = "This email address already exists!";
                    return View(user); // Redirect to the same page or handle as needed
                }

                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = $"{user.Id}_{user.First_Name.Replace(" ", "_")}{Path.GetExtension(file.FileName)}";
                    string userImagePath = Path.Combine(wwwRootPath, "image");

                    // Delete existing image if it exists
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        string existingImagePath = Path.Combine(wwwRootPath, user.Image.Substring(1)); // Remove leading backslash
                        if (System.IO.File.Exists(existingImagePath))
                        {
                            System.IO.File.Delete(existingImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(userImagePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    user.Image = @"\image\" + fileName;
                }

                _context.Update(user);
                TempData["successmsg"] = "User updated successfully!!";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [AdminAccess]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Account()
        {
            var id = HttpContext.Session.GetInt32("userId");
            if (id == null || _context.Users == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var uid = HttpContext.Session.GetInt32("userId");
            if (uid == null || _context.Users == null)
            {
                return RedirectToAction("Account", "Users");
            }
            if (uid == 1)
            {
                if (id == 1)
                {
                    TempData["errormsg"] = "Admin cannot be deleted!";
                    return RedirectToAction("Index", "Users");
                }

                var userrecord = await _context.Users
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (userrecord == null)
                {
                    return NotFound();
                }

                return View(userrecord);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == uid);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DbModel.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                TempData["successmsg"] = "User deleted successfully!!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.SingleOrDefaultAsync
                    (m => m.Emailaddress == model.Emailaddress && m.Password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("userId", user.Id);
                    HttpContext.Session.SetString("userImg", user.Image);
                    HttpContext.Session.SetString("address", user.Address);
                    if (TempData.ContainsKey("returnUrl"))
                    {
                        string returnUrl = TempData["returnUrl"].ToString();
                        TempData.Remove("returnUrl");
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Account", "Users");
                    }
                }
                ModelState.AddModelError("Password", "Invalid Login attempt");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
