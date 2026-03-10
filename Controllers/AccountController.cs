using Microsoft.AspNetCore.Mvc;
using Clothes_shop.Models.ViewModels;

namespace Clothes_shop.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewmodel model)
        {
            // Here you would typically add code to create a new user account
            // For example, you might save the user information to a database
            // After successful registration, redirect to the login page
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Here you would typically add code to authenticate the user
            // For example, you might check the user's credentials against a database
            // After successful login, redirect to the home page or user dashboard
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
