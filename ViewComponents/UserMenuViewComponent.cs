using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Clothes_shop.Models;

public class UserMenuViewComponent : ViewComponent
{
    private readonly UserManager<Users> _userManager;

    public UserMenuViewComponent(UserManager<Users> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View("LoggedIn", user);
        }

        return View("Guest");
    }
}
