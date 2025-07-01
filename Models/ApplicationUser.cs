using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MyGameList.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public ICollection<UserGameList> GameLists { get; set; } = new List<UserGameList>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Bio { get; set; }
        public bool IsProfilePublic { get; set; } = true;
        public string? AvatarPath { get; set; }
        public string? BannerPath { get; set; }

    }
}