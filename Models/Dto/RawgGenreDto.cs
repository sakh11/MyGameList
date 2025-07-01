namespace MyGameList.Models.Dto
{
    public class RawgGenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RawgGenreResponse
    {
        public List<RawgGenreDto> Results { get; set; }
    }
}
