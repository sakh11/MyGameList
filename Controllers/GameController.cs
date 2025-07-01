using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGameList.Models;
using MyGameList.Models.Dto;
using MyGameList.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyGameList.Controllers
{
    public class GameController : Controller
    {
        private readonly IRawgService _rawgService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityLogger _activityLogger;

        public GameController(
            IRawgService rawgService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IActivityLogger activityLogger)
        {
            _rawgService = rawgService;
            _context = context;
            _userManager = userManager;
            _activityLogger = activityLogger;
        }

        [HttpGet]
        public IActionResult Search(string? query)
        {
            ViewBag.Query = null;
            return View(new List<RawgGameDto>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchPost(string? query)
        {
            ViewBag.Query = query;

            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.Error = "Please enter a search term.";
                return View("Search", new List<RawgGameDto>());
            }

            var results = await _rawgService.SearchGamesAsync(query);
            return View("Search", results);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToList(int id, string title, string coverImageUrl, string releaseDate, string status, string returnUrl)
        {
            var userId = _userManager.GetUserId(User);

            var existingGame = await _context.Games
                .Include(g => g.GameGenres)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (existingGame == null)
            {
                existingGame = new Game
                {
                    Id = id, // ← THIS LINE IS NECESSARY
                    Title = title,
                    CoverImageUrl = coverImageUrl,
                    ReleaseDate = DateTime.TryParse(releaseDate, null, DateTimeStyles.RoundtripKind, out var parsed)
                        ? parsed : null,
                    GameGenres = new List<GameGenre>()
                };

                var genreDtos = await _rawgService.GetGenresForGameAsync(id);

                foreach (var genreDto in genreDtos)
                {
                    var genre = await _context.Genres.FindAsync(genreDto.Id);
                    if (genre == null)
                    {
                        genre = new Genre
                        {
                            Id = genreDto.Id,
                            Name = genreDto.Name
                        };
                        _context.Genres.Add(genre);
                    }

                    existingGame.GameGenres.Add(new GameGenre
                    {
                        GameId = id,
                        GenreId = genreDto.Id
                    });
                }

                _context.Games.Add(existingGame);
                await _context.SaveChangesAsync();
                await _activityLogger.LogAsync(userId, $"Added <strong>{title}</strong> to <em>{status}</em> list.");
            }

            // 🔐 Add to user's list if not already present
            var alreadyExists = _context.UserGameLists
                .Any(x => x.UserId == userId && x.GameId == id);

            if (!alreadyExists)
            {
                _context.UserGameLists.Add(new UserGameList
                {
                    UserId = userId,
                    GameId = id,
                    Status = status, // ✅ use the submitted value
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                TempData["Success"] = $"\"{title}\" was added to your {status} list.";
            }
            else
            {
                TempData["Success"] = $"\"{title}\" is already in your list.";
            }

            return LocalRedirect(returnUrl ?? "/");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var gameDetails = await _rawgService.GetGameDetailsAsync(id);
            if (gameDetails == null)
                return NotFound();
            
            var screenshots = await _rawgService.GetScreenshotsForGameAsync(id);
            ViewBag.Screenshots = screenshots;

            var trailers = await _rawgService.GetTrailersForGameAsync(id);
            ViewBag.Trailers = trailers;


            return View(gameDetails);
        }
    }
}