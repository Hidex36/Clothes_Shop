using Clothes_shop.Data;
using Clothes_shop.Models;
using Clothes_shop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clothes_shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;

        public AccountController(AppDbContext context,UserManager<Users> userManager,SignInManager<Users> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // ================= REGISTER =================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewmodel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new Users
            {
                UserName = model.Email,
                Email = model.Email,
                CreatedAt =DateTime.Now
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Product");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        // ================= LOGIN =================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");

            return View(model);
        }

        // ================= LOGOUT =================

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Product");
        }
        // ================= PROFILE =================

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile(string? edit)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            ViewBag.EditField = edit;

            return View(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(Users model)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Sex = model.Sex;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Message"] = "Cập nhật thành công";
            }

            return RedirectToAction("Profile");
        }


        // ================= UPDATE AVATAR =================

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            if (avatar != null && avatar.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }

                user.AvatarImageUrl = "/images/avatars/" + fileName;

                await userManager.UpdateAsync(user);
            }

            return RedirectToAction("Profile");
        }

        //Wishlist
        [HttpPost]
        public IActionResult Toggle(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var item = _context.WishLists
                .FirstOrDefault(x => x.ProductId == productId && x.UserId == userId);

            if (item == null)
            {
                _context.WishLists.Add(new WishList
                {
                    ProductId = productId,
                    UserId = userId
                });
            }
            else
            {
                _context.WishLists.Remove(item);
            }

            _context.SaveChanges();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Wishlist()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var products = _context.WishLists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .Select(w => w.Product)
                .ToList();

            return View(products);
        }
    }
}
