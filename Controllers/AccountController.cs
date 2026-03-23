using Clothes_shop.Data;
using Clothes_shop.Models;
using Clothes_shop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothes_shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;

        public AccountController(
            AppDbContext context,
            UserManager<Users> userManager,
            SignInManager<Users> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // ================= REGISTER =================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewmodel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new Users
            {
                UserName = model.Email,
                Email = model.Email,
                CreatedAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                await userManager.AddToRoleAsync(user, "User");

                await signInManager.SignOutAsync();
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
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email không tồn tại");
                return View(model);
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, false);

                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Product", new { area = "Admin" });
                }
                else if (await userManager.IsInRoleAsync(user, "Staff"))
                {
                    return RedirectToAction("Index", "Product", new { area = "Admin" });
                }

                // user thường
                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Sai mật khẩu");
            return View(model);
        }

        // ================= LOGOUT =================

        [HttpPost]
        [Authorize]
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
                var fileName = Guid.NewGuid() + Path.GetExtension(avatar.FileName);

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

        // ================= WISHLIST =================

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Toggle(int productId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            var item = _context.WishLists
                .FirstOrDefault(x => x.ProductId == productId && x.UserId == user.Id);

            if (item == null)
            {
                _context.WishLists.Add(new WishList
                {
                    ProductId = productId,
                    UserId = user.Id
                });
            }
            else
            {
                _context.WishLists.Remove(item);
            }

            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [Authorize]
        public async Task<IActionResult> Wishlist()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            var products = await _context.WishLists
                .Where(w => w.UserId == user.Id)
                .Include(w => w.Product)
                .Select(w => w.Product)
                .ToListAsync();

            return View(products);
        }
    }
}