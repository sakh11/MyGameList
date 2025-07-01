using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyGameList.Models;
using MyGameList.Models.ViewModels;
using System.Threading.Tasks;

namespace MyGameList.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SettingsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                DisplayName = user.DisplayName,
                Email = user.Email
            };

            return View(model); // this view renders both partials
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileInfo(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ProfileInfoPartial", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.DisplayName = model.DisplayName;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                ViewData["ProfileUpdateSuccess"] = "Profile updated!";
            else
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

            return PartialView("_ProfileInfoPartial", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangePasswordPartial", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                ViewData["PasswordChangeSuccess"] = "Password changed successfully.";
                ModelState.Clear();
                return PartialView("_ChangePasswordPartial", new ChangePasswordViewModel());
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return PartialView("_ChangePasswordPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(IFormFile? avatar, IFormFile? banner)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            string wwwRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (avatar != null && avatar.Length > 0)
            {
                string avatarPath = $"/uploads/avatars/{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
                string savePath = Path.Combine(wwwRoot, avatarPath.TrimStart('/'));
                using var stream = new FileStream(savePath, FileMode.Create);
                await avatar.CopyToAsync(stream);
                user.AvatarPath = avatarPath;
            }

            if (banner != null && banner.Length > 0)
            {
                string bannerPath = $"/uploads/banners/{Guid.NewGuid()}{Path.GetExtension(banner.FileName)}";
                string savePath = Path.Combine(wwwRoot, bannerPath.TrimStart('/'));
                using var stream = new FileStream(savePath, FileMode.Create);
                await banner.CopyToAsync(stream);
                user.BannerPath = bannerPath;
            }

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Images updated!";
            return RedirectToAction("Index");
        }
    }
}
