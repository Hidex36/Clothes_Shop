using Microsoft.AspNetCore.Mvc;
using Clothes_shop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Clothes_shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
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
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var Result = await userManager.CreateAsync(user, model.Password);
                if (Result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Product");
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
                var Result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (Result.Succeeded)
                {
                    return RedirectToAction("Index", "Product");
                }
                ModelState.AddModelError(string.Empty, "Sai mat khau");
            }
            return View(model);
        }
    }
}
