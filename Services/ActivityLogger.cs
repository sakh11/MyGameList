using System;
using System.Threading.Tasks;
using MyGameList.Models;

namespace MyGameList.Services
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, string description)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            _context.UserActivities.Add(activity);
            await _context.SaveChangesAsync();
        }
    }
}
