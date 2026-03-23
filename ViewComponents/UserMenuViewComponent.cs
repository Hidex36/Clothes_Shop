using Clothes_shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


public class UserMenuViewComponent : ViewComponent
{
    private readonly UserManager<Users> _userManager;
    private readonly SignInManager<Users> signInManager;

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
