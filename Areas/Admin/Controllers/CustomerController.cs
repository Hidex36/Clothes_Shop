using Clothes_shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly UserManager<Users> _userManager;

        public CustomerController(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var staffs = new List<Users>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    staffs.Add(user);
                }
            }

            return View(staffs);
        }
    }
}
