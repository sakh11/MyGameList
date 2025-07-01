using Microsoft.Extensions.Configuration;
using MyGameList.Models.Dto;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyGameList.Services
{
    public class RawgService : IRawgService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public RawgService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Rawg:ApiKey"];
            _baseUrl = "https://api.rawg.io/api";
        }


        public async Task<List<RawgGameDto>> SearchGamesAsync(string query)
        {
            var url = $"{_baseUrl}/games?key={_apiKey}&search={query}&page_size=10";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Results ?? new List<RawgGameDto>();
        }

        public async Task<List<RawgGenreDto>> GetGenresAsync()
        {
            var url = $"{_baseUrl}/genres?key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGenreResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Results ?? new List<RawgGenreDto>();
        }

        public async Task<List<RawgGenreDto>> GetGenresForGameAsync(int gameId)
        {
            var url = $"{_baseUrl}/games/{gameId}?key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var genres = new List<RawgGenreDto>();

            if (doc.RootElement.TryGetProperty("genres", out var genresElement))
            {
                foreach (var genre in genresElement.EnumerateArray())
                {
                    genres.Add(new RawgGenreDto
                    {
                        Id = genre.GetProperty("id").GetInt32(),
                        Name = genre.GetProperty("name").GetString() ?? "Unknown"
                    });
                }
            }

            return genres;
        }

        public async Task<RawgGameDetailsDto?> GetGameDetailsAsync(int id)
        {
            var url = $"games/{id}?key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return new RawgGameDetailsDto
            {
                Id = id,
                Name = root.GetProperty("name").GetString() ?? "Unknown",
                DescriptionRaw = root.GetProperty("description_raw").GetString() ?? "",
                BackgroundImage = root.GetProperty("background_image").GetString() ?? "",
                BackgroundImageAdditional = root.GetProperty("background_image_additional").GetString() ?? "",
                Released = root.TryGetProperty("released", out var released) && DateTime.TryParse(released.GetString(), out var r) ? r : null,
                Rating = root.TryGetProperty("rating", out var rating) ? rating.GetDouble() : null,
                Genres = root.GetProperty("genres").EnumerateArray().Select(g => g.GetProperty("name").GetString() ?? "").ToList(),
                Platforms = root.GetProperty("platforms")
                    .EnumerateArray()
                    .Select(p => p.GetProperty("platform").GetProperty("name").GetString() ?? "")
                    .ToList()
            };
        }

        public async Task<List<string>> GetScreenshotsForGameAsync(int gameId)
        {
            var url = $"{_baseUrl}/games/{gameId}/screenshots?key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonSerializer.Deserialize<JsonElement>(json);

            return parsed.GetProperty("results")
                .EnumerateArray()
                .Select(s => s.GetProperty("image").GetString()!)
                .ToList();
        }

        public async Task<List<string>> GetTrailersForGameAsync(int gameId)
        {
            var url = $"{_baseUrl}/games/{gameId}/movies?key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            return parsed.RootElement.GetProperty("results")
                .EnumerateArray()
                .Select(m => m.GetProperty("data").GetProperty("480").GetString()!)
                .Where(u => !string.IsNullOrEmpty(u))
                .ToList();
        }
    }
}
