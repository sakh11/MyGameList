
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameList.Models;
using MyGameList.Models.ViewModels;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new EditProfileViewModel
        {
            DisplayName = user.DisplayName
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.DisplayName = model.DisplayName;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = "Profile updated!";
        return RedirectToAction("Edit");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ViewProfile()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var user = await _userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.DisplayName })
            .FirstOrDefaultAsync();

        if (user == null || string.IsNullOrWhiteSpace(user.DisplayName))
        {
            return RedirectToAction("Login", "Account"); // or show error page
        }

        return RedirectToAction("View", new { displayName = user.DisplayName });
    }

    [AllowAnonymous]
    [HttpGet("Profile/View/{displayName}")]
    public async Task<IActionResult> View(string displayName)
    {
        var user = await _userManager.Users
            .Include(u => u.GameLists)
                .ThenInclude(ugl => ugl.Game)
                    .ThenInclude(g => g.GameGenres)
                        .ThenInclude(gg => gg.Genre)
            .FirstOrDefaultAsync(u => u.DisplayName.ToLower() == displayName.ToLower());

        if (user == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        bool isOwner = currentUserId == user.Id;

        var activities = await _context.UserActivities
            .Where(a => a.UserId == user.Id)
            .OrderByDescending(a => a.Timestamp)
            .Take(10)
            .ToListAsync();

        var model = new ViewProfileViewModel
        {
            DisplayName = user.DisplayName,
            Email = user.Email,
            Bio = user.Bio,
            CreatedAt = user.CreatedAt,
            GameLists = user.GameLists?.ToList() ?? new(),
            Activities = await _context.UserActivities
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToListAsync(),

            AvatarPath = user.AvatarPath,
            BannerPath = user.BannerPath
        };


        ViewData["IsOwner"] = isOwner;
        return View("ViewProfile", model);
    }
}