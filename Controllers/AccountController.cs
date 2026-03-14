using Microsoft.AspNetCore.Mvc;
using Clothes_shop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Clothes_shop.Models; // Đảm bảo namespace này chứa class Users của bạn

namespace Clothes_shop.Controllers
{
    public class AccountController : Controller
    {
        // Thay đổi từ IdentityUser sang Users
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;

        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewmodel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    UserName = model.Email,
                    Email = model.Email,
                    CreatedAt = DateTime.Now
                };

                var Result = await userManager.CreateAsync(user, model.Password);
                if (Result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Product");
                }

                // Hiển thị lỗi từ Identity nếu đăng ký thất bại (ví dụ mật khẩu yếu)
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Identity mặc định dùng UserName để đăng nhập, ở đây bạn đang gán UserName = Email
                var Result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false,lockoutOnFailure: false);

                if (Result.Succeeded)
                {
                    return RedirectToAction("Index", "Product");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }
    }
}