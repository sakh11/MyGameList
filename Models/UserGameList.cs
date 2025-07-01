using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGameList.Models
{
    public class UserGameList
    {
        public int Id { get; set; }

        // Foreign Key to Identity User
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        // Foreign Key to Game
        public int GameId { get; set; }
        public Game Game { get; set; }

        // Backlog / Playing / Completed / Wishlist
        [Required]
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
