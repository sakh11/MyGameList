using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // <-- Add this

namespace MyGameList.Models
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // <-- This line tells EF not to auto-generate Id
        public int Id { get; set; } // RAWG genre ID

        public string Name { get; set; }

        public ICollection<GameGenre> GameGenres { get; set; }
    }
}
