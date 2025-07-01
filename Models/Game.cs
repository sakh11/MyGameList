using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyGameList.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // ✅ Important!
        public int Id { get; set; }

        public string Title { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public ICollection<UserGameList> UserGameLists { get; set; }
        public ICollection<GameGenre> GameGenres { get; set; }
    }
}