using System;
using System.Collections.Generic;
using MyGameList.Models;

namespace MyGameList.Models.ViewModels
{
    public class ViewProfileViewModel
    {
        public string DisplayName { get; set; } = "";
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; } = "";
        public List<UserGameList> GameLists { get; set; } = new();
        public List<UserActivity>? Activities { get; set; }
        public string? AvatarPath { get; set; }
        public string? BannerPath { get; set; }
    }
}
