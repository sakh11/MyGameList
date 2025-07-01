using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameList.Models;
using MyGameList.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGameList.Controllers
{
    [Authorize]
    public class GameListController : Controller
    {
        private readonly IRawgService _rawgService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameListController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IRawgService rawgService)
        {
            _context = context;
            _userManager = userManager;
            _rawgService = rawgService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string name, int? year)
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.UserGameLists
                .Include(x => x.Game)
                .Where(x => x.UserId == userId);

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.Game.Title.Contains(name));

            if (year.HasValue)
                query = query.Where(x => x.Game.ReleaseDate.HasValue && x.Game.ReleaseDate.Value.Year == year.Value);

            var results = await query.ToListAsync();

            // ✅ This is what's missing or broken right now
            ViewBag.Genres = await _rawgService.GetGenresAsync();

            return View(results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int gameId)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.UserGameLists
                .FirstOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId);

            if (item != null)
            {
                _context.UserGameLists.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"\"{item.Game?.Title ?? "Game"}\" was removed from your list.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Section(string status, string name, int? year, string sort, int? genreId)
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.UserGameLists
                .Include(x => x.Game)
                    .ThenInclude(g => g.GameGenres)
                        .ThenInclude(gg => gg.Genre)
                .Where(x => x.UserId == userId && x.Status == status);

            // Filters
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.Game.Title.Contains(name));

            if (year.HasValue)
                query = query.Where(x => x.Game.ReleaseDate.HasValue && x.Game.ReleaseDate.Value.Year == year.Value);

            // Sorting
            if (sort == "title")
                query = query.OrderBy(x => x.Game.Title);
            else if (sort == "date")
                query = query.OrderByDescending(x => x.Game.ReleaseDate);

            if (genreId.HasValue)
            {
                query = query
                    .Where(x => x.Game.GameGenres.Any(gg => gg.GenreId == genreId));
            }


            var list = await query.ToListAsync();
            ViewData["SectionTitle"] = status;
            ViewData["IsOwner"] = true; // ✅ Important

            return PartialView("_GameListSection", list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int gameId, string newStatus)
        {
            var userId = _userManager.GetUserId(User);
            var entry = await _context.UserGameLists
                .FirstOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId);

            if (entry != null && !string.IsNullOrWhiteSpace(newStatus))
            {
                entry.Status = newStatus;
                await _context.SaveChangesAsync();
            }

            return Ok(); // Called via AJAX
        }

    }
}
