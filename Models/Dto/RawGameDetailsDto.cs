using System;
using System.Collections.Generic;

namespace MyGameList.Models.Dto
{
    public class RawgGameDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DescriptionRaw { get; set; }
        public string BackgroundImage { get; set; }
        public string BackgroundImageAdditional { get; set; }
        public DateTime? Released { get; set; }
        public double? Rating { get; set; }

        public List<string> Genres { get; set; } = new();
        public List<string> Platforms { get; set; } = new();
    }
}
