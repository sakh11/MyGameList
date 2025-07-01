using System.Collections.Generic;
using System.Threading.Tasks;
using MyGameList.Models.Dto;

namespace MyGameList.Services
{
    public interface IRawgService
    {
        Task<List<RawgGameDto>> SearchGamesAsync(string query);

        Task<List<RawgGenreDto>> GetGenresAsync();

        Task<List<RawgGenreDto>> GetGenresForGameAsync(int gameId);

        Task<RawgGameDetailsDto?> GetGameDetailsAsync(int id);
        
        Task<List<string>> GetScreenshotsForGameAsync(int gameId);
        
        Task<List<string>> GetTrailersForGameAsync(int gameId);


    }
}
