using Clothes_shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clothes_shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager; // Đổi IdentityUser thành Users

        // CHỈ DÙNG 1 HÀM KHỞI TẠO NÀY
        public StaffController(UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager; // Bây giờ nó sẽ không bị Null nữa
        }

        // ================= LIST STAFF =================
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var staffs = new List<Users>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Staff"))
                {
                    staffs.Add(user);
                }
            }

            return View(staffs);
        }

        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Users model, string password, IFormFile avatarFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔥 Upload ảnh
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                model.AvatarImageUrl = "/images/" + fileName;
            }

            model.UserName = model.Email;
            model.CreatedAt = DateTime.Now;

            var result = await _userManager.CreateAsync(model, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(model, "Staff");
                return RedirectToAction("Index");
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }

            return View(model);
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        //LOGOUT
        [HttpPost]
        [AllowAnonymous] // Cho phép thoát ngay cả khi token hết hạn
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        // ================= EDIT =================

        // GET: Admin/Staff/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            return View(user);
        }


        // POST: Admin/Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Users model, string password)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
                return NotFound();

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Sex = model.Sex;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.AvatarImageUrl = model.AvatarImageUrl;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var err in updateResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View(model);
            }
            if (!string.IsNullOrEmpty(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, password);

                if (!passResult.Succeeded)
                {
                    foreach (var err in passResult.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View(model);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
