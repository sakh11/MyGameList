using System;
using System.Collections.Generic;

namespace MyGameList.Models.Dto
{
    public class RawgGameDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Released { get; set; }
        public string Background_Image { get; set; }
    }

    public class RawgApiResponse
    {
        public List<RawgGameDto> Results { get; set; }
    }
}